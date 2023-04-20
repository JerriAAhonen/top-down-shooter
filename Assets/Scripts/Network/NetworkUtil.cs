using Unity.Netcode;
using UA = UnityEngine.Assertions;

public static class NetworkUtil
{
	public static class Assert
	{
		public static void IsHost()
		{
			UA.Assert.IsTrue(NetworkManager.Singleton.IsHost);
		}
		
		public static void IsClient()
		{
			UA.Assert.IsTrue(NetworkManager.Singleton.IsClient);
		}
	}
}
