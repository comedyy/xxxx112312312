public struct PositionInputStruct : IInputStruct
{
    public fp x;
    public fp z;
    public bool isSingtonInput => true;
    public int structType => (int)InputType.Position;
}