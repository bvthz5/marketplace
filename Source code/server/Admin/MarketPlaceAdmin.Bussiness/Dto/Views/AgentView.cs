using MarketPlace.DataAccess.Model;

namespace MarketPlaceAdmin.Bussiness.Dto.Views
{
    public class AgentView
    {
        public int AgentId { get; }
        public string Name { get; }
        public string PhoneNumber { get; }
        public string Email { get; }
        public byte Status { get; }
        public DateTime CreatedDate { get; }

        public AgentView(Agent agent)
        {
            AgentId = agent.AgentId;
            Name = agent.Name;
            PhoneNumber = agent.PhoneNumber;
            Email = agent.Email;
            Status = (byte) agent.Status;
            CreatedDate = agent.CreatedDate;

        }
    }

    public class AgentDetailView : AgentView
    {
        public DateTime UpdateDate { get; set; }
        public string? ProfilePic { get; set; }


        public AgentDetailView(Agent agent) : base(agent)
        {
            UpdateDate = agent.UpdatedDate;
            ProfilePic = agent.ProfilePic;

        }
    }
}
