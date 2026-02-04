using CAMCMSServer.Database.Context;
using CAMCMSServer.Model;
using CAMCMSServer.Utils;

namespace CAMCMSServer.Database.Repository
{
    public interface IInvitationRepository
    {
        Task Add(Invitation invitation);
        Task<Invitation> GetById(int invitationId);
        Task<IEnumerable<Invitation>> GetAll();
        Task Update(Invitation invitation);
        Task Delete(int invitationId);
    }

    public class InvitationRepository : IInvitationRepository
    {
        private readonly IDataContext context;

        public InvitationRepository(IDataContext context)
        {
            this.context = context;
        }

        public async Task Add(Invitation invitation)
        {
            if (invitation == null)
            {
                throw new ArgumentNullException(nameof(invitation));
            }

            using var connection = await this.context.CreateConnection();
            await connection.ExecuteAsync(SqlConstants.InsertInvitation, invitation);
        }

        public async Task<Invitation> GetById(int invitationId)
        {
            if (invitationId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(invitationId));
            }

            using var connection = await this.context.CreateConnection();
            var invitations = await connection.QueryAsync<Invitation>(SqlConstants.SelectInvitationById, new { Id = invitationId });
            return invitations.FirstOrDefault();
        }

        public async Task<IEnumerable<Invitation>> GetAll()
        {
            using var connection = await this.context.CreateConnection();
            var invitations = await connection.QueryAsync<Invitation>(SqlConstants.SelectAllInvitations);
            return invitations;
        }

        public async Task Update(Invitation invitation)
        {
            if (invitation == null)
            {
                throw new ArgumentNullException(nameof(invitation));
            }

            using var connection = await this.context.CreateConnection();
            await connection.ExecuteAsync(SqlConstants.UpdateInvitation, invitation);
        }

        public async Task Delete(int invitationId)
        {
            if (invitationId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(invitationId));
            }

            using var connection = await this.context.CreateConnection();
            await connection.ExecuteAsync(SqlConstants.DeleteInvitation, new { Id = invitationId });
        }
    }
}

