using System.Collections.Generic;
using System.Linq;
using FProj.Api;
using FProj.Data;
using FProj.Repository.Base;

namespace FProj.Repository
{
    public class ActorRepository: BaseRepository, IFetch<ActorApi>
    {
        public ActorRepository(FProjContext context) : base(context)
        {
        }

        public List<ActorApi> GetAll(bool IsDeleted = false)
        {
            return _dbContext.Actor.Select(DataToApi.ActorToApi).ToList();
        }
    }
}
