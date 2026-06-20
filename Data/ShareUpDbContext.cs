using Microsoft.EntityFrameworkCore;
using ShareUp.API.Entities;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace ShareUp.API.Data;

public class ShareUpDbContext : DbContext
{
  public ShareUpDbContext(DbContextOptions<ShareUpDbContext> options) : base(options) { }

  public DbSet<Group> Groups { get; set; }
  public DbSet<Member> Members { get; set; }
  public DbSet<Expense> Expenses { get; set; }
  public DbSet<ExpenseShare> ExpenseShares { get; set; }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);

    // 1. Group -> Members (Cascade)
    modelBuilder.Entity<Member>()
        .HasOne<Group>() // هنا تمام لأن كلاس Member مافيهوش Navigation لـ Group
        .WithMany(g => g.Members)
        .HasForeignKey(m => m.GroupId)
        .OnDelete(DeleteBehavior.Cascade);

    // 2. Group -> Expenses (Cascade)
    modelBuilder.Entity<Expense>()
        .HasOne<Group>() // هنا تمام لأن كلاس Expense مافيهوش Navigation لـ Group
        .WithMany(g => g.Expenses)
        .HasForeignKey(e => e.GroupId)
        .OnDelete(DeleteBehavior.Cascade);

    // 3. Expense -> ExpenseShares (Cascade)
    modelBuilder.Entity<ExpenseShare>()
        .HasOne(es => es.Expense) // ⭐ التعديل هنا: شاورنا على الـ Property المكتوبة في الكلاس
        .WithMany(e => e.ExpenseShares)
        .HasForeignKey(es => es.ExpenseId)
        .OnDelete(DeleteBehavior.Cascade);

    // 4. Expense -> PaidBy Member (Restrict)
    modelBuilder.Entity<Expense>()
        .HasOne<Member>()
        .WithMany(m => m.Expenses) // تأكدي إن كلاس Member جواه ICollection<Expense> Expenses
        .HasForeignKey(e => e.PaidById)
        .OnDelete(DeleteBehavior.Restrict);

    // 5. ExpenseShare -> Member (Restrict)
    modelBuilder.Entity<ExpenseShare>()
        .HasOne(es => es.Member) // ⭐ التعديل هنا: شاورنا على الـ Property المكتوبة في الكلاس
        .WithMany(m => m.ExpenseShares) // تأكدي إن كلاس Member جواه ICollection<ExpenseShare> ExpenseShares لو كنتِ عاملاها، لو مش عاملاها شيلي ما بداخل WithMany() وسيبها فاضية
        .HasForeignKey(es => es.MemberId)
        .OnDelete(DeleteBehavior.Restrict);
  }
}
