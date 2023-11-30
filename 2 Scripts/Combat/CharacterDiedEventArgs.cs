using System;

public class CharacterDiedEventArgs : EventArgs
{
    public CharacterDiedEventArgs(Character character)
    {
        Character = character;
    }

    public Character Character { get; }
}