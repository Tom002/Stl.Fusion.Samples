using System;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Stl.CommandR.Configuration;
using Stl.Fusion;
using Stl.Fusion.Authentication;

namespace Samples.BoardGames.Abstractions
{
    // API-level entities

    public enum GameStage
    {
        Created = 0,
        Running = 1,
        Ended = 0x10,
    }

    public record Game
    {
        public record CreateCommand(Session Session, string Type) : ISessionCommand<Game> {
            public CreateCommand() : this(Session.Null, "") { }
        }
        public record JoinCommand(Session Session, string Id) : ISessionCommand {
            public JoinCommand() : this(Session.Null, "") { }
        }
        public record MoveCommand(Session Session, string Id, GameMove Move) : ISessionCommand {
            public MoveCommand() : this(Session.Null, "", null!) { }
        }
        public record EditCommand(Session Session, string Id, bool IsPublic) : ISessionCommand {
            public EditCommand() : this(Session.Null, "", false) { }
        }

        public string Id { get; init; } = "";
        public string Type { get; init; } = "";
        public bool IsPublic { get; init; }
        public ImmutableList<GamePlayer> Players { get; init; } = ImmutableList<GamePlayer>.Empty;
        public DateTime CreatedAt { get; init; }
        public DateTime? StartedAt { get; init; }
        public DateTime? EndedAt { get; init; }
        public GameState? State { get; init; }
        public GameStage Stage { get; init; }
    }

    public record GamePlayer
    {
        public long UserId { get; set; }
        public long Score { get; set; }
    }

    // Services

    public interface IGameService
    {
        [CommandHandler]
        Task<Game> CreateAsync(Game.CreateCommand command, CancellationToken cancellationToken = default);
        [CommandHandler]
        Task JoinAsync(Game.JoinCommand command, CancellationToken cancellationToken = default);
        [CommandHandler]
        Task MoveAsync(Game.MoveCommand command, CancellationToken cancellationToken = default);
        [CommandHandler]
        Task EditAsync(Game.EditCommand command, CancellationToken cancellationToken = default);

        [ComputeMethod(KeepAliveTime = 1)]
        Task<Game?> FindAsync(string id, Session session, CancellationToken cancellationToken = default);
    }
}
