using Microsoft.EntityFrameworkCore;
using JsonPlaceholderClone.Models;

namespace JsonPlaceholderClone.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Post> Posts { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<Album> Albums { get; set; }
    public DbSet<Photo> Photos { get; set; }
    public DbSet<Todo> Todos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure User entity
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Username).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Email).IsRequired();
            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasIndex(e => e.Username).IsUnique();
            
            // Configure Address as owned entity type
            entity.OwnsOne(e => e.Address, address =>
            {
                address.Property(a => a.Street).IsRequired().HasMaxLength(200);
                address.Property(a => a.Suite).IsRequired().HasMaxLength(100);
                address.Property(a => a.City).IsRequired().HasMaxLength(100);
                address.Property(a => a.Zipcode).IsRequired().HasMaxLength(20);
                
                // Configure Geo as owned entity type within Address
                address.OwnsOne(a => a.Geo, geo =>
                {
                    geo.Property(g => g.Lat).IsRequired();
                    geo.Property(g => g.Lng).IsRequired();
                });
            });
            
            // Configure Company as owned entity type
            entity.OwnsOne(e => e.Company, company =>
            {
                company.Property(c => c.Name).IsRequired().HasMaxLength(100);
                company.Property(c => c.CatchPhrase).HasMaxLength(200);
                company.Property(c => c.Bs).HasMaxLength(100);
            });
        });

        // Configure Post entity
        modelBuilder.Entity<Post>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Body).IsRequired();
            entity.HasOne(e => e.User)
                  .WithMany(u => u.Posts)
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure Comment entity
        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Email).IsRequired();
            entity.Property(e => e.Body).IsRequired();
            entity.HasOne(e => e.Post)
                  .WithMany(p => p.Comments)
                  .HasForeignKey(e => e.PostId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure Album entity
        modelBuilder.Entity<Album>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.HasOne(e => e.User)
                  .WithMany(u => u.Albums)
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure Photo entity
        modelBuilder.Entity<Photo>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Url).IsRequired();
            entity.Property(e => e.ThumbnailUrl).IsRequired();
            entity.HasOne(e => e.Album)
                  .WithMany(a => a.Photos)
                  .HasForeignKey(e => e.AlbumId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure Todo entity
        modelBuilder.Entity<Todo>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.HasOne(e => e.User)
                  .WithMany(u => u.Todos)
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Seed data - temporarily disabled due to owned entity configuration
        // SeedData(modelBuilder);
    }

    private void SeedData(ModelBuilder modelBuilder)
    {
        // Seed Users
        var users = new List<User>
        {
            new() { Id = 1, Name = "Leanne Graham", Username = "Bret", Email = "Sincere@april.biz", Phone = "1-770-736-8031 x56442", Website = "hildegard.org" },
            new() { Id = 2, Name = "Ervin Howell", Username = "Antonette", Email = "Shanna@melissa.tv", Phone = "010-692-6593 x09125", Website = "anastasia.net" },
            new() { Id = 3, Name = "Clementine Bauch", Username = "Samantha", Email = "Nathan@yesenia.net", Phone = "1-463-123-4447", Website = "ramiro.info" },
            new() { Id = 4, Name = "Patricia Lebsack", Username = "Karianne", Email = "Julianne.OConner@kory.org", Phone = "493-170-9623 x156", Website = "kale.biz" },
            new() { Id = 5, Name = "Chelsey Dietrich", Username = "Kamren", Email = "Lucio_Hettinger@annie.ca", Phone = "(254)954-1289", Website = "demarco.info" },
            new() { Id = 6, Name = "Mrs. Dennis Schulist", Username = "Leopoldo_Corkery", Email = "Karley_Dach@jasper.info", Phone = "1-477-935-8478 x6430", Website = "ola.org" },
            new() { Id = 7, Name = "Kurtis Weissnat", Username = "Elwyn.Skiles", Email = "Telly.Hoeger@billy.biz", Phone = "210.067.6132", Website = "elvis.io" },
            new() { Id = 8, Name = "Nicholas Runolfsdottir V", Username = "Maxime_Nienow", Email = "Sherwood@rosamond.me", Phone = "586.493.6943 x140", Website = "jacynthe.com" },
            new() { Id = 9, Name = "Glenna Reichert", Username = "Delphine", Email = "Chaim_McDermott@dana.io", Phone = "(775)976-6794 x41206", Website = "conrad.com" },
            new() { Id = 10, Name = "Clementina DuBuque", Username = "Moriah.Stanton", Email = "Rey.Padberg@karina.biz", Phone = "024-648-3804", Website = "ambrose.net" }
        };

        modelBuilder.Entity<User>().HasData(users);

        // Seed Posts (first 20 posts)
        var posts = new List<Post>();
        for (int i = 1; i <= 20; i++)
        {
            posts.Add(new Post
            {
                Id = i,
                UserId = ((i - 1) % 10) + 1,
                Title = $"Post Title {i}",
                Body = $"This is the body content for post {i}. It contains some sample text to demonstrate the API functionality."
            });
        }
        modelBuilder.Entity<Post>().HasData(posts);

        // Seed Comments (first 50 comments)
        var comments = new List<Comment>();
        for (int i = 1; i <= 50; i++)
        {
            comments.Add(new Comment
            {
                Id = i,
                PostId = ((i - 1) % 20) + 1,
                Name = $"Commenter {i}",
                Email = $"commenter{i}@example.com",
                Body = $"This is comment {i} on post {((i - 1) % 20) + 1}. Great post!"
            });
        }
        modelBuilder.Entity<Comment>().HasData(comments);

        // Seed Albums (first 10 albums)
        var albums = new List<Album>();
        for (int i = 1; i <= 10; i++)
        {
            albums.Add(new Album
            {
                Id = i,
                UserId = i,
                Title = $"Album {i}"
            });
        }
        modelBuilder.Entity<Album>().HasData(albums);

        // Seed Photos (first 50 photos)
        var photos = new List<Photo>();
        for (int i = 1; i <= 50; i++)
        {
            photos.Add(new Photo
            {
                Id = i,
                AlbumId = ((i - 1) % 10) + 1,
                Title = $"Photo {i}",
                Url = $"https://via.placeholder.com/600/92c952",
                ThumbnailUrl = $"https://via.placeholder.com/150/92c952"
            });
        }
        modelBuilder.Entity<Photo>().HasData(photos);

        // Seed Todos (first 20 todos)
        var todos = new List<Todo>();
        for (int i = 1; i <= 20; i++)
        {
            todos.Add(new Todo
            {
                Id = i,
                UserId = ((i - 1) % 10) + 1,
                Title = $"Todo {i}",
                Completed = i % 2 == 0
            });
        }
        modelBuilder.Entity<Todo>().HasData(todos);
    }
} 