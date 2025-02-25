// using CareerCompass.Infrastructure.Common;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.EntityFrameworkCore.Metadata.Builders;
//
// namespace Playground;
//
// class User
// {
//     private readonly List<Post> _posts = [];
//     public int Id { get; set; }
//     public string Name { get; set; }
//
//     // [BackingField(nameof(_posts))]
//     public IReadOnlyList<Post> Posts => _posts.AsReadOnly();
//
//     public void AddPost(string title, string content)
//     {
//         var post = new Post
//         {
//             // Id = _posts.Count + 1,
//             Title = title,
//             Content = content,
//         };
//
//         _posts.Add(post);
//     }
//
//
//     public void UpdatePost(int id, string title, string content)
//     {
//         var existingPost = _posts.First(x => x.Id == id);
//         existingPost.Title = title;
//         existingPost.Content = content;
//     }
//
//     public void RemovePost(int id)
//     {
//         var post = _posts.First(x => x.Id == id);
//         _posts.Remove(post);
//     }
// }
//
// class Post
// {
//     public int Id { get; set; }
//     public string Title { get; set; }
//     public string Content { get; set; }
//
//     public int UserId { get; set; }
// }
//
// class TestDbContext : DbContext
// {
//     public TestDbContext(DbContextOptions<TestDbContext> options) : base(options)
//     {
//     }
//
//     protected override void OnModelCreating(ModelBuilder modelBuilder)
//     {
//         modelBuilder.ApplyConfigurationsFromAssembly(typeof(Program).Assembly);
//     }
//
//     public DbSet<User> Users { get; set; }
// }
//
// class UserConfiguration : IEntityTypeConfiguration<User>
// {
//     public void Configure(EntityTypeBuilder<User> builder)
//     {
//         builder.ToTable("Users");
//         // builder.HasKey(e => e.Id);
//         builder.Property(e => e.Name)
//             .HasMaxLength(500)
//             .IsRequired();
//
//         builder.OwnsMany(e => e.Posts, pb =>
//         {
//             pb.ToTable("Posts");
//             pb.WithOwner()
//                 .HasForeignKey(nameof(Post.UserId));
//
//             // pb.HasKey(nameof(Post.Id));
//             pb.Property(e => e.Title)
//                 .HasMaxLength(500)
//                 .IsRequired();
//             pb.Property(e => e.Content)
//                 .HasMaxLength(500)
//                 .IsRequired();
//         });
//
//         var nav = builder.Metadata.FindNavigation(nameof(User.Posts));
//         if (nav is null)
//         {
//             throw new DatabaseOperationException(
//                 $"{nameof(UserConfiguration)}: Navigation property ( {nameof(User.Posts)} ) was not found");
//         }
//
//         nav.SetPropertyAccessMode(PropertyAccessMode.Field);
//     }
// }