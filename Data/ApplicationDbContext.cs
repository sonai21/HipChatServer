using HipChatServer.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace HipChatServer.Data;

public class ApplicationDbContext: DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Chat> Chats { get; set; }
    public DbSet<Message> Messages { get; set; }
   
}
