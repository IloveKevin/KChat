using Config;
using EFCore.Entity;
using KChatServe.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Util.Enum;

namespace Service.Implement
{
	public class MessageService : IMessageService
	{
		private readonly MySqlServerDataBaseContext _mySqlServerDataBaseContext;
		private readonly IOptions<ChatConfigration> _chatOption;

		public MessageService(MySqlServerDataBaseContext mySqlServerDataBaseContext,IOptions<ChatConfigration> chatoption)
		{
			_mySqlServerDataBaseContext = mySqlServerDataBaseContext;
			_chatOption = chatoption;
		}
		public async Task<Message> AddMessage(long SenderId, long receiverId, string content)
		{
			var message = new Message
			{
				SenderId = SenderId,
				ReceiverId = receiverId,
				Content = content,
				SendTime = DateTime.Now,
				Status = EChatMessageStatus.Waiting,
			};
			_mySqlServerDataBaseContext._messages.Add(message);
			await _mySqlServerDataBaseContext.SaveChangesAsync();
			return message;
		}

		public async Task<List<Message>> GetHistoryMessages(long userId, long friendId, long start, int size)
		{
		   return await _mySqlServerDataBaseContext._messages.Where(x => (x.SenderId == userId && x.ReceiverId == friendId) || (x.SenderId == friendId && x.ReceiverId == userId) && x.Id < start).OrderBy(x => x.Id).Take(size).ToListAsync();
		}

		public async Task<List<Message>> GetUnReadMessages(long userId)
		{
			return await _mySqlServerDataBaseContext._messages.Where(x => x.ReceiverId == userId && x.Status == EChatMessageStatus.Waiting).ToListAsync();
		}

		public async Task<Message?> ReadMessage(long userId,long messageId)
		{
			var message = await _mySqlServerDataBaseContext._messages.FirstOrDefaultAsync(x => x.Id == messageId&& x.ReceiverId == userId &&x.Status!=EChatMessageStatus.Seen);
			if (message == null)
			{
				return null;
			}
			message.Status = EChatMessageStatus.Seen;
			await _mySqlServerDataBaseContext.SaveChangesAsync();
			return message;
		}

		public async Task<Message?> RetractMessage(long userId, long messageId)
		{
			var message = await _mySqlServerDataBaseContext._messages.FirstOrDefaultAsync(x => x.Id == messageId && x.SenderId==userId && (DateTime.Now - x.SendTime).TotalMilliseconds <= _chatOption.Value.RetractTime * 60 * 1000);
			if(message == null)
			{
				return null;
			}
			message.Status = EChatMessageStatus.Retracted;
			return message;
		}

		public async Task SendMessage(long messageId)
		{
			var message = await _mySqlServerDataBaseContext._messages.FirstOrDefaultAsync(x => x.Id == messageId && x.Status == EChatMessageStatus.Waiting);
			if (message == null)
			{
				return;
			}
			message.Status = EChatMessageStatus.UnSeen;
			await _mySqlServerDataBaseContext.SaveChangesAsync();
		}
	}
}
