using EFCore.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interface
{
	public interface IMessageService
	{
		Task<Message> AddMessage(long SenderId, long receiverId, string content);
		Task<List<Message>> GetHistoryMessages(long userId,long friendId,long start,int size);
		Task<List<Message>> GetUnReadMessages(long userId);
		Task SendMessage(long messageId);
		Task<Message?> ReadMessage(long userId,long messageId);
		Task<Message?> RetractMessage(long userId,long messageId);
	}
}
