
namespace DalamudSystem.Extensions
{
    public static class ListExtension
    {
        public static void DalamudSystemMoveListItem<T>(this IList<T> List, int Position, MoveDirection Direction)
        {
            int Modifier = Direction == MoveDirection.Up ? -1 : 1;
            int Positional = Position + Modifier;
            if (Positional < 0 || Positional >= List.Count) return;
            (List[Position], List[Position + Modifier]) = (List[Position + Modifier], List[Position]);
        }
    }

    public enum MoveDirection
    {
        Up,
        Down
    }
}
