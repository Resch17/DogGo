using DogGo.Models;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;

namespace DogGo.Repositories
{
    public interface IWalkRepository
    {
        List<Walk> GetAllWalks();
        List<Walk> GetWalksByWalkerId(int walkerId);
        void AddWalk(Walk walk);
    }
}