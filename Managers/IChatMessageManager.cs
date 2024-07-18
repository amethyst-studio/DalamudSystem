using DalamudSystem.Manager;
using FFXIVClientStructs.FFXIV.Client.System.Memory;
using FFXIVClientStructs.FFXIV.Client.System.String;
using System.Runtime.InteropServices;
using System.Text;
using Framework = FFXIVClientStructs.FFXIV.Client.System.Framework.Framework;

namespace DalamudSystem.Managers
{

    /// <summary>
    /// Class used to send chat messages to the client.
    /// 
    /// This class is UNSTABLE and DANGEROUS to use. Please use with extreme caution.
    /// </summary>
    public unsafe class IChatMessageManager : IManager, IDisposable
    {
        #region Memory Scanner Magic - Finds and Exposes Additional APIs that Dalamud does not provide.
        private static class SignatureTable
        {
            internal const string SendChat = "48 89 5C 24 ?? 57 48 83 EC 20 48 8B FA 48 8B D9 45 84 C9";
            internal const string SanitizeString = "E8 ?? ?? ?? ?? 48 8D 4C 24 ?? 0F B6 F0 E8 ?? ?? ?? ?? 48 8D 4D C0";
        }

        private delegate void ProcessChatBoxDelegate(IntPtr ui, IntPtr message, IntPtr unused, byte a4);
        private ProcessChatBoxDelegate? ProcessChatBox { get; }

        private readonly unsafe delegate* unmanaged<Utf8String*, int, IntPtr, void> _sanitize = null!;

        internal IChatMessageManager() : base(nameof(IChatMessageManager))
        {
            if (ICoreManager.SigScanner.TryScanText(SignatureTable.SendChat, out var processChatBoxPtr))
            {
                this.ProcessChatBox = Marshal.GetDelegateForFunctionPointer<ProcessChatBoxDelegate>(processChatBoxPtr);
            }

            if (ICoreManager.SigScanner.TryScanText(SignatureTable.SanitizeString, out var SanitizePtr))
            {
                this._sanitize = (delegate* unmanaged<Utf8String*, int, IntPtr, void>)SanitizePtr;
            }
        }
        #endregion

        /// <summary>
        /// This method is more inherintly safe but is limited on validation that can be provided. Please use InvokeChatCommand when possible to better protect the players.
        /// </summary>
        /// <param name="ChatMessage">String to send to current chat.</param>
        /// <exception cref="InvalidOperationException">If memory address lookup failed.</exception>
        /// <exception cref="ArgumentException">If the message is empty, exceeds 500 bytes, or does not match sanitization.</exception>
        public void InvokeChatMessage(string ChatMessage)
        {
            if (ProcessChatBox == null) throw new InvalidOperationException("Could not find signature for chat process box. Please report this to DalamudSystem repo.");

            ChatMessage = ChatMessage.Trim();
            var bytes = Encoding.UTF8.GetBytes(ChatMessage);
            if (bytes.Length == 0 || bytes.Length > 500) throw new ArgumentException($"Attempted to invoke a 'Text Message' but the 'ChatMessage' was invalid: {ChatMessage}");
            if (ChatMessage.Length != SanitizeText(ChatMessage).Length) throw new ArgumentException($"Attempted to invoke a 'Text Command' but the 'ChatMessage' contained illegal characters: Provided = '{ChatMessage}' | Parsed = '{SanitizeText(ChatMessage)}'");

            var ui = (IntPtr)Framework.Instance()->GetUIModule();
            using var payload = new ChatPayload(bytes);
            var memstore = Marshal.AllocHGlobal(400);
            Marshal.StructureToPtr(payload, memstore, false);
            this.ProcessChatBox(ui, memstore, IntPtr.Zero, 0);
            Marshal.FreeHGlobal(memstore);
        }

        /// <summary>
        /// This method is considered better to use over InvokeChatMessage. Sending to chat is preferred to use client commands within validation bounds.
        /// </summary>
        /// <param name="ChatMessage"></param>
        /// <exception cref="InvalidOperationException">If memory address lookup failed or no prefix is provided to the ChatMessage.</exception>
        /// <exception cref="ArgumentException">If the message is empty, exceeds 500 bytes, or does not match sanitization.</exception>
        public void InvokeChatCommand(string ChatMessage)
        {
            ChatMessage = ChatMessage.Trim();
            if (!ChatMessage.StartsWith("/")) throw new InvalidOperationException($"Attempted to invoke a 'Slash Command' but was improperly prefixed with '/': {ChatMessage}");
            InvokeChatMessage(ChatMessage);
        }

        /// <summary>
        /// Internal method to call SQEX built-in sanitization functions.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        private unsafe string SanitizeText(string text)
        {
            if (_sanitize == null) throw new InvalidOperationException("Could not find signature for chat sanitization. Please report this to DalamudSystem repo.");

            // Convert to UTF8
            var utf8Text = Utf8String.FromString(text);
            _sanitize(utf8Text, 0x27F, IntPtr.Zero);

            // PConvert utf8Text to String and Dtor(?)
            var sanitized = utf8Text->ToString();
            utf8Text->Dtor();
            
            // Clean Memory
            IMemorySpace.Free(utf8Text);

            // Return Result
            return sanitized;
        }

        // ChatPayload Struct
        [StructLayout(LayoutKind.Explicit)]
        private readonly struct ChatPayload : IDisposable
        {
            [FieldOffset(0)]
            private readonly IntPtr textPtr;

            [FieldOffset(16)]
            private readonly ulong textLen;

            [FieldOffset(8)]
            private readonly ulong unk1;

            [FieldOffset(24)]
            private readonly ulong unk2;

            internal ChatPayload(byte[] stringBytes)
            {
                this.textPtr = Marshal.AllocHGlobal(stringBytes.Length + 30);
                Marshal.Copy(stringBytes, 0, this.textPtr, stringBytes.Length);
                Marshal.WriteByte(this.textPtr + stringBytes.Length, 0);

                this.textLen = (ulong)(stringBytes.Length + 1);

                this.unk1 = 64;
                this.unk2 = 0;
            }

            public void Dispose()
            {
                Marshal.FreeHGlobal(this.textPtr);
            }
        }

        public override void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
