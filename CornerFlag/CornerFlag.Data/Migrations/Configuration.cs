namespace CornerFlag.Data.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    using CornerFlag.Data;
    using CornerFlag.Data.Models.Entities;
    using CornerFlag.Data.Models.People;
    using CornerFlag.Common;

    internal sealed class Configuration : DbMigrationsConfiguration<CornerFlagDbContext>
    {
        private const int PLAYERS_COUNT = 160;
        private const int CLUBS_COUNT = 16;
        private const int COMPETITIONS_COUNT = 4;
        private const int COUNTRIES_COUNT = 2;

        public Configuration()
        {
            this.AutomaticMigrationsEnabled = true;
            this.AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(CornerFlagDbContext context)
        {
            context.Configuration.AutoDetectChangesEnabled = false;
            this.SeedCountries(context);
            this.SeedPlayers(context);
            this.SeedClubs(context);
            this.SeedCompetitions(context);
            this.SeedCompsClubsRelations(context);

            context.Configuration.AutoDetectChangesEnabled = true;
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //
        }

        private void SeedCompsClubsRelations(CornerFlagDbContext context)
        {
            var countries = context.Countries.ToList();
            foreach (var country in countries)
            {
                var competitions = context.Competitions.Where(comp => comp.Country.Id == country.Id).ToList();
                foreach (var comp in competitions)
                {
                    var c = country;
                    var avaliableClubs = context.Clubs.Where(cl => cl.Country.Id == c.Id && cl.Competitions.Count == 0).ToList();
                    foreach (var club in avaliableClubs)
                    {
                        club.Competitions.Add(comp);
                        comp.Clubs.Add(club);
                        context.SaveChanges();
                    }
                }
            }
        }

        private void SeedCompetitions(CornerFlagDbContext context)
        {
            var generator = new RandomDataGenerator();
            for (int i = 0; i < COMPETITIONS_COUNT; i++)
            {
                var competition = new Competition();
                competition.Name = generator.GetString(2, 20);
                var country = context.Countries.First(c => c.Competitions.Count < COUNTRIES_COUNT);
                competition.Country = country;

                country.Competitions.Add(competition);
                context.Competitions.Add(competition);
                context.SaveChanges();
            }
        }

        private void SeedClubs(CornerFlagDbContext context)
        {
            var generator = new RandomDataGenerator();
            var from = DateTime.Now.AddYears(-40);
            var to = DateTime.Now.AddYears(-10);
            var currentPlayerIndex = 0;
            var players = context.Players.ToList();
            for (int i = 0; i < CLUBS_COUNT; i++)
            {
                var club = new Club();
                club.Name = generator.GetString(2, 20);
                club.DateFounded = generator.GetRandomDate(from, to);
                var country = context.Countries.First(c => c.Clubs.Count < (COUNTRIES_COUNT * COMPETITIONS_COUNT));
                club.Country = country;
                var team = new Team();
                team.Name = "First Team";
                for (int j = 0; j < 10; j++)
                {
                    team.Players.Add(players[currentPlayerIndex]);
                    currentPlayerIndex++;
                }

                context.Teams.Add(team);
                club.Teams.Add(team);
                country.Clubs.Add(club);
                context.Clubs.AddOrUpdate(club);
                context.SaveChanges();
            }
        }

        private void SeedCountries(CornerFlagDbContext context)
        {
            int countriesCount = 10;
            var generator = new RandomDataGenerator();
            for (int i = 0; i < countriesCount; i++)
            {
                var country = new Country();
                country.Name = generator.GetString(2, 20);
                context.Countries.AddOrUpdate(country);
            }

            context.SaveChanges();
        }

        private void SeedPlayers(CornerFlagDbContext context)
        {
            int playersCount = PLAYERS_COUNT;
            var countries = context.Countries;
            var generator = new RandomDataGenerator();
            var from = DateTime.Now.AddYears(-40);
            var to = DateTime.Now.AddYears(-10);
            for (int i = 0; i < playersCount; i++)
            {
                var player = new Player();
                player.BirthDate = generator.GetRandomDate(from, to);
                player.FirstName = generator.GetString(2, 20);
                player.LastName = generator.GetString(2, 20);
                player.Position = GetPosition(i);
                var countryId = generator.GetInt(1, 10);
                player.Country = countries.First(c => c.Id == countryId);
                context.Players.AddOrUpdate(player);
                if (i % 100 == 0)
                {
                    context.SaveChanges();
                }
            }

            context.SaveChanges();
        }

        private Position GetPosition(int i)
        {
            switch (i % 10)
            {
                case 0:
                    return Position.Goalkeeper;
                case 1:
                case 2:
                case 3:
                case 4:
                    return Position.Midfielder;
                case 5:
                case 6:
                case 7:
                case 8:
                    return Position.Defender;
                case 9:
                    return Position.Forward;
                default:
                    break;
            }

            return Position.Midfielder;
        }
    }
}
