// File: PlayerDataMessage.cs
using Mirror;

// Make sure the struct is public so it can be accessed from other files
public struct PlayerDataMessage : NetworkMessage
{
	public int selectedCarIndex;  // The car index the client selected
}
