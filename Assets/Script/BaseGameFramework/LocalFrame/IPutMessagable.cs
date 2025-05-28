using System.Collections.Generic;

public interface IPutMessage
{
    void AddLocalFrame(int frame, UserFrameInput? item);
    void AddFrameWithList(int frame, List<UserFrameInput> item);
    public int ReceivedServerFrame { get;}
}