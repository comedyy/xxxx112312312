using System;

public class InputStructFactory
{
    internal static IInputStruct Create(InputType type)
    {
        switch (type)
        {
            case InputType.Position:
                return new UserPositionInput();
            // Add more cases for other input types as needed
            default:
                throw new ArgumentException($"Unsupported InputType: {type}");
        }
    }
}