using FairShareSolutions_PYMT.Infrastructure.ViewModel;
using FairShareSolutions_PYMT.Model.IRepository.IDashboardRepository;
using Microsoft.AspNetCore.SignalR;
using Serilog;
using System.Web.Mvc;

namespace FairShareSolutions_PYMT.API.SignalR
{
	[AllowAnonymous]
	public class GetItems : Hub
	{
		private readonly IGroupDetailRepository _dashboardRepository;
		private static Dictionary<string, string> _connections = new Dictionary<string, string>();
		public GetItems(IGroupDetailRepository dashboardRepository)
		{
			_dashboardRepository = dashboardRepository;
		}
		public override async Task OnConnectedAsync()
		{
			// Log or process the connection (e.g., storing connection ID)
			string connectionId = Context.ConnectionId;
			string userName = "Unknown";

			Log.Information("SignalR client connected. ConnectionId: {ConnectionId}", connectionId);

			_connections[connectionId] = userName;

			Log.Information("Connection registered. ConnectionId: {ConnectionId}, UserName: {UserName}", connectionId, userName);

			// Send a welcome message to the newly connected client
			await Clients.Caller.SendAsync("ReceiveMessage", "System", $"Welcome {userName}! You are connected.");

			// Optionally broadcast to all other clients that a new user has connected
			await Clients.All.SendAsync("ReceiveMessage", "System", $"{userName} has joined the chat!");

			Log.Information("Broadcasted client join notification. ConnectionId: {ConnectionId}", connectionId);
			await base.OnConnectedAsync();
		}
		public override async Task OnDisconnectedAsync(Exception? exception)
		{
			string connectionId = Context.ConnectionId;
			Log.Information("SignalR client disconnected. ConnectionId: {ConnectionId}", connectionId);

			// Remove the user from the active connection list
			if (_connections.ContainsKey(connectionId))
			{
				Log.Information("Connection removed. ConnectionId: {ConnectionId}", connectionId);
				string userName = _connections[connectionId];
				_connections.Remove(connectionId);

				// Notify all other clients that the user has disconnected
				await Clients.All.SendAsync("ReceiveMessage", "System", $"{userName} has left the chat.");

				Log.Information("Broadcasted client disconnect notification. ConnectionId: {ConnectionId}", connectionId);
			}

			await base.OnDisconnectedAsync(exception);
		}

		public async Task SendMessage(int GroupId, string UserId, int ItemId, decimal SelectedQuantity)
		//public async Task SendMessage(SelectItemsSplitViewModel selectItemSplitViewModel)
		{
			Log.Information("SendMessage invoked. GroupId: {GroupId}, UserId: {UserId}, ItemId: {ItemId}, SelectedQuantity: {SelectedQuantity}", GroupId, UserId, ItemId, SelectedQuantity);

			ChatMessage QuantityData = await _dashboardRepository.SendMessage(GroupId, UserId, ItemId, SelectedQuantity);
			//ChatMessage QuantityData = await _dashboardRepository.SendMessageUpdate(GroupId, UserId, ItemId, SelectedQuantity,selectItemSplitViewModel);

			Log.Information("Repository processed message. GroupId: {GroupId}, UserId: {UserId}, ItemId: {ItemId}, RemainingQuantity: {RemainingQuantity}, TotalQuantity: {TotalQuantity}", GroupId, UserId, ItemId, QuantityData.RemainingQuantity, QuantityData.TotalQuantity);

			ChatMessage data = new ChatMessage();
			data.GroupId = GroupId;
			data.UserId = UserId;
			data.ItemId = ItemId;
			data.SelectedQuantity = SelectedQuantity;
			data.RemainingQuantity = QuantityData.RemainingQuantity;
			data.TotalQuantity = QuantityData.TotalQuantity;

			// await Clients.Group($"user-{UserId}").SendAsync("ReceiveMessage", data);
			// await Clients.Group($"user-{GroupId}").SendAsync("ReceiveMessage", data);

			await Clients.All.SendAsync("ReceiveMessage", GroupId, UserId, ItemId, SelectedQuantity, QuantityData.RemainingQuantity, QuantityData.TotalQuantity);

			Log.Information("Message broadcast completed. GroupId: {GroupId}, UserId: {UserId}, ItemId: {ItemId}", GroupId, UserId, ItemId);

			//// Send to receiver (if connected)
			//await Clients.User(toUserId).SendAsync("ReceiveMessage", fromUserId, message);

			//// Optionally echo back to sender
			//await Clients.User(fromUserId).SendAsync("ReceiveMessage", toUserId, message);
		}

		public async Task JoinGroup(int groupId)
		{
			//  Connection to the requested SignalR group
			await Groups.AddToGroupAsync(	Context.ConnectionId,$"group-{groupId}");

			Log.Information( "Client joined group. GroupId: {GroupId}", groupId);

			// Fetch current group details
			var details =
				await _dashboardRepository.GetGroupDetails(groupId);

			//  Send the details to this client 
			Log.Information($"Sending group details to caller. GroupId: {groupId}, Details: {details}");
			await Clients.Caller.SendAsync("ReceiveGroupDetails",	details);
		}

		public async Task RefreshGroupDetails(int groupId)
		{
			Log.Information("Client refreshed group. GroupId: {GroupId}", groupId);
			var details =await _dashboardRepository.GetGroupDetails(groupId);

			await Clients.Group($"group-{groupId}").SendAsync("ReceiveGroupDetails", details);
		}

	}

}
