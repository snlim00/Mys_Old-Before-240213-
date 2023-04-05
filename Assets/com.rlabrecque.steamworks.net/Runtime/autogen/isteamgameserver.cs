// This file is provided under The MIT License as part of Steamworks.NET.
// Copyright (c) 2013-2022 Riley Labrecque
// Please see the included LICENSE.txt for additional information.

// This file is automatically generated.
// Changes to this file will be reverted when you update Steamworks.NET

#if !(UNITY_STANDALONE_WIN || UNITY_STANDALONE_LINUX || UNITY_STANDALONE_OSX || STEAMWORKS_WIN || STEAMWORKS_LIN_OSX)
	#define DISABLESTEAMWORKS
#endif

#if !DISABLESTEAMWORKS

using System.Runtime.InteropServices;
using IntPtr = System.IntPtr;

namespace Steamworks {
	public static class SteamGameServer {
		/// <summary>
		/// <para>/ Game product identifier.  This is currently used by the master server for version checking purposes.</para>
		/// <para>/ It's a required field, but will eventually will go away, and the AppID will be used for this purpose.</para>
		/// </summary>
		public static void SetProduct(string pszProduct) {
			InteropHelp.TestIfAvailableGameServer();
			using (var pszProduct2 = new InteropHelp.UTF8StringHandle(pszProduct)) {
				NativeMethods.ISteamGameServer_SetProduct(CSteamGameServerAPIContext.GetSteamGameServer(), pszProduct2);
			}
		}

		/// <summary>
		/// <para>/ Description of the game.  This is a required field and is displayed in the steam server browser....for now.</para>
		/// <para>/ This is a required field, but it will go away eventually, as the data should be determined from the AppID.</para>
		/// </summary>
		public static void SetGameDescription(string pszGameDescription) {
			InteropHelp.TestIfAvailableGameServer();
			using (var pszGameDescription2 = new InteropHelp.UTF8StringHandle(pszGameDescription)) {
				NativeMethods.ISteamGameServer_SetGameDescription(CSteamGameServerAPIContext.GetSteamGameServer(), pszGameDescription2);
			}
		}

		/// <summary>
		/// <para>/ If your game is a "mod," pass the string that identifies it.  The default is an empty string, meaning</para>
		/// <para>/ this application is the original game, not a mod.</para>
		/// <para>/</para>
		/// <para>/ @see k_cbMaxGameServerGameDir</para>
		/// </summary>
		public static void SetModDir(string pszModDir) {
			InteropHelp.TestIfAvailableGameServer();
			using (var pszModDir2 = new InteropHelp.UTF8StringHandle(pszModDir)) {
				NativeMethods.ISteamGameServer_SetModDir(CSteamGameServerAPIContext.GetSteamGameServer(), pszModDir2);
			}
		}

		/// <summary>
		/// <para>/ Is this is a dedicated server?  The default value is false.</para>
		/// </summary>
		public static void SetDedicatedServer(bool bDedicated) {
			InteropHelp.TestIfAvailableGameServer();
			NativeMethods.ISteamGameServer_SetDedicatedServer(CSteamGameServerAPIContext.GetSteamGameServer(), bDedicated);
		}

		/// <summary>
		/// <para> Login</para>
		/// <para>/ Begin process to login to a persistent game server account</para>
		/// <para>/</para>
		/// <para>/ You need to register for callbacks to determine the result of this operation.</para>
		/// <para>/ @see SteamServersConnected_t</para>
		/// <para>/ @see SteamServerConnectFailure_t</para>
		/// <para>/ @see SteamServersDisconnected_t</para>
		/// </summary>
		public static void LogOn(string pszToken) {
			InteropHelp.TestIfAvailableGameServer();
			using (var pszToken2 = new InteropHelp.UTF8StringHandle(pszToken)) {
				NativeMethods.ISteamGameServer_LogOn(CSteamGameServerAPIContext.GetSteamGameServer(), pszToken2);
			}
		}

		/// <summary>
		/// <para>/ Login to a generic, anonymous account.</para>
		/// <para>/</para>
		/// <para>/ Note: in previous versions of the SDK, this was automatically called within SteamGameServer_Init,</para>
		/// <para>/ but this is no longer the case.</para>
		/// </summary>
		public static void LogOnAnonymous() {
			InteropHelp.TestIfAvailableGameServer();
			NativeMethods.ISteamGameServer_LogOnAnonymous(CSteamGameServerAPIContext.GetSteamGameServer());
		}

		/// <summary>
		/// <para>/ Begin process of logging game server out of steam</para>
		/// </summary>
		public static void LogOff() {
			InteropHelp.TestIfAvailableGameServer();
			NativeMethods.ISteamGameServer_LogOff(CSteamGameServerAPIContext.GetSteamGameServer());
		}

		/// <summary>
		/// <para> status functions</para>
		/// </summary>
		public static bool BLoggedOn() {
			InteropHelp.TestIfAvailableGameServer();
			return NativeMethods.ISteamGameServer_BLoggedOn(CSteamGameServerAPIContext.GetSteamGameServer());
		}

		public static bool BSecure() {
			InteropHelp.TestIfAvailableGameServer();
			return NativeMethods.ISteamGameServer_BSecure(CSteamGameServerAPIContext.GetSteamGameServer());
		}

		public static CSteamID GetSteamID() {
			InteropHelp.TestIfAvailableGameServer();
			return (CSteamID)NativeMethods.ISteamGameServer_GetSteamID(CSteamGameServerAPIContext.GetSteamGameServer());
		}

		/// <summary>
		/// <para>/ Returns true if the master server has requested a restart.</para>
		/// <para>/ Only returns true once per request.</para>
		/// </summary>
		public static bool WasRestartRequested() {
			InteropHelp.TestIfAvailableGameServer();
			return NativeMethods.ISteamGameServer_WasRestartRequested(CSteamGameServerAPIContext.GetSteamGameServer());
		}

		/// <summary>
		/// <para> Server state.  These properties may be changed at any time.</para>
		/// <para>/ Max player count that will be reported to server browser and client queries</para>
		/// </summary>
		public static void SetMaxPlayerCount(int cPlayersMax) {
			InteropHelp.TestIfAvailableGameServer();
			NativeMethods.ISteamGameServer_SetMaxPlayerCount(CSteamGameServerAPIContext.GetSteamGameServer(), cPlayersMax);
		}

		/// <summary>
		/// <para>/ Number of bots.  Default value is zero</para>
		/// </summary>
		public static void SetBotPlayerCount(int cBotplayers) {
			InteropHelp.TestIfAvailableGameServer();
			NativeMethods.ISteamGameServer_SetBotPlayerCount(CSteamGameServerAPIContext.GetSteamGameServer(), cBotplayers);
		}

		/// <summary>
		/// <para>/ Set the name of server as it will appear in the server browser</para>
		/// <para>/</para>
		/// <para>/ @see k_cbMaxGameServerName</para>
		/// </summary>
		public static void SetServerName(string pszServerName) {
			InteropHelp.TestIfAvailableGameServer();
			using (var pszServerName2 = new InteropHelp.UTF8StringHandle(pszServerName)) {
				NativeMethods.ISteamGameServer_SetServerName(CSteamGameServerAPIContext.GetSteamGameServer(), pszServerName2);
			}
		}

		/// <summary>
		/// <para>/ Set name of map to report in the server browser</para>
		/// <para>/</para>
		/// <para>/ @see k_cbMaxGameServerMapName</para>
		/// </summary>
		public static void SetMapName(string pszMapName) {
			InteropHelp.TestIfAvailableGameServer();
			using (var pszMapName2 = new InteropHelp.UTF8StringHandle(pszMapName)) {
				NativeMethods.ISteamGameServer_SetMapName(CSteamGameServerAPIContext.GetSteamGameServer(), pszMapName2);
			}
		}

		/// <summary>
		/// <para>/ Let people know if your server will require a password</para>
		/// </summary>
		public static void SetPasswordProtected(bool bPasswordProtected) {
			InteropHelp.TestIfAvailableGameServer();
			NativeMethods.ISteamGameServer_SetPasswordProtected(CSteamGameServerAPIContext.GetSteamGameServer(), bPasswordProtected);
		}

		/// <summary>
		/// <para>/ Spectator server port to advertise.  The default value is zero, meaning the</para>
		/// <para>/ service is not used.  If your server receives any info requests on the LAN,</para>
		/// <para>/ this is the value that will be placed into the reply for such local queries.</para>
		/// <para>/</para>
		/// <para>/ This is also the value that will be advertised by the master server.</para>
		/// <para>/ The only exception is if your server is using a FakeIP.  Then then the second</para>
		/// <para>/ fake port number (index 1) assigned to your server will be listed on the master</para>
		/// <para>/ server as the spectator port, if you set this value to any nonzero value.</para>
		/// <para>/</para>
		/// <para>/ This function merely controls the values that are advertised -- it's up to you to</para>
		/// <para>/ configure the server to actually listen on this port and handle any spectator traffic</para>
		/// </summary>
		public static void SetSpectatorPort(ushort unSpectatorPort) {
			InteropHelp.TestIfAvailableGameServer();
			NativeMethods.ISteamGameServer_SetSpectatorPort(CSteamGameServerAPIContext.GetSteamGameServer(), unSpectatorPort);
		}

		/// <summary>
		/// <para>/ Name of the spectator server.  (Only used if spectator port is nonzero.)</para>
		/// <para>/</para>
		/// <para>/ @see k_cbMaxGameServerMapName</para>
		/// </summary>
		public static void SetSpectatorServerName(string pszSpectatorServerName) {
			InteropHelp.TestIfAvailableGameServer();
			using (var pszSpectatorServerName2 = new InteropHelp.UTF8StringHandle(pszSpectatorServerName)) {
				NativeMethods.ISteamGameServer_SetSpectatorServerName(CSteamGameServerAPIContext.GetSteamGameServer(), pszSpectatorServerName2);
			}
		}

		/// <summary>
		/// <para>/ Call this to clear the whole list of key/values that are sent in rules queries.</para>
		/// </summary>
		public static void ClearAllKeyValues() {
			InteropHelp.TestIfAvailableGameServer();
			NativeMethods.ISteamGameServer_ClearAllKeyValues(CSteamGameServerAPIContext.GetSteamGameServer());
		}

		/// <summary>
		/// <para>/ Call this to add/update a key/value pair.</para>
		/// </summary>
		public static void SetKeyValue(string pKey, string pValue) {
			InteropHelp.TestIfAvailableGameServer();
			using (var pKey2 = new InteropHelp.UTF8StringHandle(pKey))
			using (var pValue2 = new InteropHelp.UTF8StringHandle(pValue)) {
				NativeMethods.ISteamGameServer_SetKeyValue(CSteamGameServerAPIContext.GetSteamGameServer(), pKey2, pValue2);
			}
		}

		/// <summary>
		/// <para>/ Sets a string defining the "gametags" for this server, this is optional, but if it is set</para>
		/// <para>/ it allows users to filter in the matchmaking/server-browser interfaces based on the value</para>
		/// <para>/</para>
		/// <para>/ @see k_cbMaxGameServerTags</para>
		/// </summary>
		public static void SetGameTags(string pchGameTags) {
			InteropHelp.TestIfAvailableGameServer();
			using (var pchGameTags2 = new InteropHelp.UTF8StringHandle(pchGameTags)) {
				NativeMethods.ISteamGameServer_SetGameTags(CSteamGameServerAPIContext.GetSteamGameServer(), pchGameTags2);
			}
		}

		/// <summary>
		/// <para>/ Sets a string defining the "gamedata" for this server, this is optional, but if it is set</para>
		/// <para>/ it allows users to filter in the matchmaking/server-browser interfaces based on the value</para>
		/// <para>/</para>
		/// <para>/ @see k_cbMaxGameServerGameData</para>
		/// </summary>
		public static void SetGameData(string pchGameData) {
			InteropHelp.TestIfAvailableGameServer();
			using (var pchGameData2 = new InteropHelp.UTF8StringHandle(pchGameData)) {
				NativeMethods.ISteamGameServer_SetGameData(CSteamGameServerAPIContext.GetSteamGameServer(), pchGameData2);
			}
		}

		/// <summary>
		/// <para>/ Region identifier.  This is an optional field, the default value is empty, meaning the "world" region</para>
		/// </summary>
		public static void SetRegion(string pszRegion) {
			InteropHelp.TestIfAvailableGameServer();
			using (var pszRegion2 = new InteropHelp.UTF8StringHandle(pszRegion)) {
				NativeMethods.ISteamGameServer_SetRegion(CSteamGameServerAPIContext.GetSteamGameServer(), pszRegion2);
			}
		}

		/// <summary>
		/// <para>/ Indicate whether you wish to be listed on the master server list</para>
		/// <para>/ and/or respond to server browser / LAN discovery packets.</para>
		/// <para>/ The server starts with this value set to false.  You should set all</para>
		/// <para>/ relevant server parameters before enabling advertisement on the server.</para>
		/// <para>/</para>
		/// <para>/ (This function used to be named EnableHeartbeats, so if you are wondering</para>
		/// <para>/ where that function went, it's right here.  It does the same thing as before,</para>
		/// <para>/ the old name was just confusing.)</para>
		/// </summary>
		public static void SetAdvertiseServerActive(bool bActive) {
			InteropHelp.TestIfAvailableGameServer();
			NativeMethods.ISteamGameServer_SetAdvertiseServerActive(CSteamGameServerAPIContext.GetSteamGameServer(), bActive);
		}

		/// <summary>
		/// <para> Player list management / authentication.</para>
		/// <para> Retrieve ticket to be sent to the entity who wishes to authenticate you ( using BeginAuthSession API ).</para>
		/// <para> pcbTicket retrieves the length of the actual ticket.</para>
		/// </summary>
		public static HAuthTicket GetAuthSessionTicket(byte[] pTicket, int cbMaxTicket, out uint pcbTicket) {
			InteropHelp.TestIfAvailableGameServer();
			return (HAuthTicket)NativeMethods.ISteamGameServer_GetAuthSessionTicket(CSteamGameServerAPIContext.GetSteamGameServer(), pTicket, cbMaxTicket, out pcbTicket);
		}

		/// <summary>
		/// <para> Authenticate ticket ( from GetAuthSessionTicket ) from entity steamID to be sure it is valid and isnt reused</para>
		/// <para> Registers for callbacks if the entity goes offline or cancels the ticket ( see ValidateAuthTicketResponse_t callback and EAuthSessionResponse )</para>
		/// </summary>
		public static EBeginAuthSessionResult BeginAuthSession(byte[] pAuthTicket, int cbAuthTicket, CSteamID steamID) {
			InteropHelp.TestIfAvailableGameServer();
			return NativeMethods.ISteamGameServer_BeginAuthSession(CSteamGameServerAPIContext.GetSteamGameServer(), pAuthTicket, cbAuthTicket, steamID);
		}

		/// <summary>
		/// <para> Stop tracking started by BeginAuthSession - called when no longer playing game with this entity</para>
		/// </summary>
		public static void EndAuthSession(CSteamID steamID) {
			InteropHelp.TestIfAvailableGameServer();
			NativeMethods.ISteamGameServer_EndAuthSession(CSteamGameServerAPIContext.GetSteamGameServer(), steamID);
		}

		/// <summary>
		/// <para> Cancel auth ticket from GetAuthSessionTicket, called when no longer playing game with the entity you gave the ticket to</para>
		/// </summary>
		public static void CancelAuthTicket(HAuthTicket hAuthTicket) {
			InteropHelp.TestIfAvailableGameServer();
			NativeMethods.ISteamGameServer_CancelAuthTicket(CSteamGameServerAPIContext.GetSteamGameServer(), hAuthTicket);
		}

		/// <summary>
		/// <para> After receiving a user's authentication data, and passing it to SendUserConnectAndAuthenticate, use this function</para>
		/// <para> to determine if the user owns downloadable content specified by the provided AppID.</para>
		/// </summary>
		public static EUserHasLicenseForAppResult UserHasLicenseForApp(CSteamID steamID, AppId_t appID) {
			InteropHelp.TestIfAvailableGameServer();
			return NativeMethods.ISteamGameServer_UserHasLicenseForApp(CSteamGameServerAPIContext.GetSteamGameServer(), steamID, appID);
		}

		/// <summary>
		/// <para> Ask if a user in in the specified group, results returns async by GSUserGroupStatus_t</para>
		/// <para> returns false if we're not connected to the steam servers and thus cannot ask</para>
		/// </summary>
		public static bool RequestUserGroupStatus(CSteamID steamIDUser, CSteamID steamIDGroup) {
			InteropHelp.TestIfAvailableGameServer();
			return NativeMethods.ISteamGameServer_RequestUserGroupStatus(CSteamGameServerAPIContext.GetSteamGameServer(), steamIDUser, steamIDGroup);
		}

		/// <summary>
		/// <para> these two functions s are deprecated, and will not return results</para>
		/// <para> they will be removed in a future version of the SDK</para>
		/// </summary>
		public static void GetGameplayStats() {
			InteropHelp.TestIfAvailableGameServer();
			NativeMethods.ISteamGameServer_GetGameplayStats(CSteamGameServerAPIContext.GetSteamGameServer());
		}

		public static SteamAPICall_t GetServerReputation() {
			InteropHelp.TestIfAvailableGameServer();
			return (SteamAPICall_t)NativeMethods.ISteamGameServer_GetServerReputation(CSteamGameServerAPIContext.GetSteamGameServer());
		}

		/// <summary>
		/// <para> Returns the public IP of the server according to Steam, useful when the server is</para>
		/// <para> behind NAT and you want to advertise its IP in a lobby for other clients to directly</para>
		/// <para> connect to</para>
		/// </summary>
		public static SteamIPAddress_t GetPublicIP() {
			InteropHelp.TestIfAvailableGameServer();
			return NativeMethods.ISteamGameServer_GetPublicIP(CSteamGameServerAPIContext.GetSteamGameServer());
		}

		/// <summary>
		/// <para> Server browser related query packet processing for shared socket mode.  These are used</para>
		/// <para> when you pass STEAMGAMESERVER_QUERY_PORT_SHARED as the query port to SteamGameServer_Init.</para>
		/// <para> IP address and port are in host order, i.e 127.0.0.1 == 0x7f000001</para>
		/// <para> These are used when you've elected to multiplex the game server's UDP socket</para>
		/// <para> rather than having the master server updater use its own sockets.</para>
		/// <para> Source games use this to simplify the job of the server admins, so they</para>
		/// <para> don't have to open up more ports on their firewalls.</para>
		/// <para> Call this when a packet that starts with 0xFFFFFFFF comes in. That means</para>
		/// <para> it's for us.</para>
		/// </summary>
		public static bool HandleIncomingPacket(byte[] pData, int cbData, uint srcIP, ushort srcPort) {
			InteropHelp.TestIfAvailableGameServer();
			return NativeMethods.ISteamGameServer_HandleIncomingPacket(CSteamGameServerAPIContext.GetSteamGameServer(), pData, cbData, srcIP, srcPort);
		}

		/// <summary>
		/// <para> AFTER calling HandleIncomingPacket for any packets that came in that frame, call this.</para>
		/// <para> This gets a packet that the master server updater needs to send out on UDP.</para>
		/// <para> It returns the length of the packet it wants to send, or 0 if there are no more packets to send.</para>
		/// <para> Call this each frame until it returns 0.</para>
		/// </summary>
		public static int GetNextOutgoingPacket(byte[] pOut, int cbMaxOut, out uint pNetAdr, out ushort pPort) {
			InteropHelp.TestIfAvailableGameServer();
			return NativeMethods.ISteamGameServer_GetNextOutgoingPacket(CSteamGameServerAPIContext.GetSteamGameServer(), pOut, cbMaxOut, out pNetAdr, out pPort);
		}

		/// <summary>
		/// <para> Server clan association</para>
		/// <para> associate this game server with this clan for the purposes of computing player compat</para>
		/// </summary>
		public static SteamAPICall_t AssociateWithClan(CSteamID steamIDClan) {
			InteropHelp.TestIfAvailableGameServer();
			return (SteamAPICall_t)NativeMethods.ISteamGameServer_AssociateWithClan(CSteamGameServerAPIContext.GetSteamGameServer(), steamIDClan);
		}

		/// <summary>
		/// <para> ask if any of the current players dont want to play with this new player - or vice versa</para>
		/// </summary>
		public static SteamAPICall_t ComputeNewPlayerCompatibility(CSteamID steamIDNewPlayer) {
			InteropHelp.TestIfAvailableGameServer();
			return (SteamAPICall_t)NativeMethods.ISteamGameServer_ComputeNewPlayerCompatibility(CSteamGameServerAPIContext.GetSteamGameServer(), steamIDNewPlayer);
		}

		/// <summary>
		/// <para> Handles receiving a new connection from a Steam user.  This call will ask the Steam</para>
		/// <para> servers to validate the users identity, app ownership, and VAC status.  If the Steam servers</para>
		/// <para> are off-line, then it will validate the cached ticket itself which will validate app ownership</para>
		/// <para> and identity.  The AuthBlob here should be acquired on the game client using SteamUser()-&gt;InitiateGameConnection()</para>
		/// <para> and must then be sent up to the game server for authentication.</para>
		/// <para> Return Value: returns true if the users ticket passes basic checks. pSteamIDUser will contain the Steam ID of this user. pSteamIDUser must NOT be NULL</para>
		/// <para> If the call succeeds then you should expect a GSClientApprove_t or GSClientDeny_t callback which will tell you whether authentication</para>
		/// <para> for the user has succeeded or failed (the steamid in the callback will match the one returned by this call)</para>
		/// <para> DEPRECATED!  This function will be removed from the SDK in an upcoming version.</para>
		/// <para>              Please migrate to BeginAuthSession and related functions.</para>
		/// </summary>
		public static bool SendUserConnectAndAuthenticate_DEPRECATED(uint unIPClient, byte[] pvAuthBlob, uint cubAuthBlobSize, out CSteamID pSteamIDUser) {
			InteropHelp.TestIfAvailableGameServer();
			return NativeMethods.ISteamGameServer_SendUserConnectAndAuthenticate_DEPRECATED(CSteamGameServerAPIContext.GetSteamGameServer(), unIPClient, pvAuthBlob, cubAuthBlobSize, out pSteamIDUser);
		}

		/// <summary>
		/// <para> Creates a fake user (ie, a bot) which will be listed as playing on the server, but skips validation.</para>
		/// <para> Return Value: Returns a SteamID for the user to be tracked with, you should call EndAuthSession()</para>
		/// <para> when this user leaves the server just like you would for a real user.</para>
		/// </summary>
		public static CSteamID CreateUnauthenticatedUserConnection() {
			InteropHelp.TestIfAvailableGameServer();
			return (CSteamID)NativeMethods.ISteamGameServer_CreateUnauthenticatedUserConnection(CSteamGameServerAPIContext.GetSteamGameServer());
		}

		/// <summary>
		/// <para> Should be called whenever a user leaves our game server, this lets Steam internally</para>
		/// <para> track which users are currently on which servers for the purposes of preventing a single</para>
		/// <para> account being logged into multiple servers, showing who is currently on a server, etc.</para>
		/// <para> DEPRECATED!  This function will be removed from the SDK in an upcoming version.</para>
		/// <para>              Please migrate to BeginAuthSession and related functions.</para>
		/// </summary>
		public static void SendUserDisconnect_DEPRECATED(CSteamID steamIDUser) {
			InteropHelp.TestIfAvailableGameServer();
			NativeMethods.ISteamGameServer_SendUserDisconnect_DEPRECATED(CSteamGameServerAPIContext.GetSteamGameServer(), steamIDUser);
		}

		/// <summary>
		/// <para> Update the data to be displayed in the server browser and matchmaking interfaces for a user</para>
		/// <para> currently connected to the server.  For regular users you must call this after you receive a</para>
		/// <para> GSUserValidationSuccess callback.</para>
		/// <para> Return Value: true if successful, false if failure (ie, steamIDUser wasn't for an active player)</para>
		/// </summary>
		public static bool BUpdateUserData(CSteamID steamIDUser, string pchPlayerName, uint uScore) {
			InteropHelp.TestIfAvailableGameServer();
			using (var pchPlayerName2 = new InteropHelp.UTF8StringHandle(pchPlayerName)) {
				return NativeMethods.ISteamGameServer_BUpdateUserData(CSteamGameServerAPIContext.GetSteamGameServer(), steamIDUser, pchPlayerName2, uScore);
			}
		}
	}
}

#endif // !DISABLESTEAMWORKS
