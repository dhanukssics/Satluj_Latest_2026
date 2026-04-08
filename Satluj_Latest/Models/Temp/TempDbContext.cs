using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Satluj_Latest.Models.Temp;

public partial class TempDbContext : DbContext
{
    public TempDbContext(DbContextOptions<TempDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<TbChatConversation> TbChatConversations { get; set; }

    public virtual DbSet<TbChatMessage> TbChatMessages { get; set; }

    public virtual DbSet<TbChatParticipant> TbChatParticipants { get; set; }

    public virtual DbSet<TbChatUser> TbChatUsers { get; set; }

    public virtual DbSet<TbSeason> TbSeasons { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TbChatConversation>(entity =>
        {
            entity.HasKey(e => e.ConversationId).HasName("PK__tb_ChatC__C050D87743CC4BDF");

            entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.TbChatConversations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ConversationCreator");
        });

        modelBuilder.Entity<TbChatMessage>(entity =>
        {
            entity.HasKey(e => e.MessageId).HasName("PK__tb_ChatM__C87C0C9CBDD28B93");

            entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Conversation).WithMany(p => p.TbChatMessages)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MessageConversation");

            entity.HasOne(d => d.FromChatUser).WithMany(p => p.TbChatMessages)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MessageSender");
        });

        modelBuilder.Entity<TbChatParticipant>(entity =>
        {
            entity.HasKey(e => new { e.ConversationId, e.ChatUserId }).HasName("PK_ChatParticipants");

            entity.Property(e => e.JoinedOn).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.ChatUser).WithMany(p => p.TbChatParticipants)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CP_User");

            entity.HasOne(d => d.Conversation).WithMany(p => p.TbChatParticipants)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CP_Conversation");
        });

        modelBuilder.Entity<TbChatUser>(entity =>
        {
            entity.HasKey(e => e.ChatUserId).HasName("PK__tb_ChatU__BFA9F790F3249218");

            entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IsOnline).HasDefaultValue(false);
        });

        modelBuilder.Entity<TbSeason>(entity =>
        {
            entity.HasKey(e => e.SeasonId).HasName("PK__tb_Seaso__C1814E38FB4C607F");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
