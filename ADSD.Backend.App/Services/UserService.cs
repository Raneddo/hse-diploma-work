using ADSD.Backend.App.Clients;
using ADSD.Backend.App.Json;

namespace ADSD.Backend.App.Services;

public class UserService
{
    private readonly AppDbClient _appDbClient;

    public UserService(AppDbClient appDbClient)
    {
        _appDbClient = appDbClient;
    }

    public IEnumerable<UserBaseInfo> GetUsers()
    {
        var users = _appDbClient.GetUsers()
            .ToList();

        foreach (var user in users)
        {
            user.Roles = _appDbClient.GetRolesByUserId(user.Id)
                .Select(x => x.ToString())
                .ToList();
            yield return user;
        }
    }

    public UserFullInfo GetUser(int id)
    {
        var user = _appDbClient.GetUserById(id);
        if (user == null)
        {
            return null;
        }
        user.Roles = _appDbClient.GetRolesByUserId(user.Id)
            .Select(x => x.ToString())
            .ToList();
        return user;
    }

    public int CreateUser(UserFullInfo userFullInfo)
    {
        var userId = _appDbClient.CreateUser(userFullInfo);
        _appDbClient.UpdateUserRoles(userId, userFullInfo.Roles);
        return userId;
    }

    public void UpdateUser(int id, UserFullInfo userFullInfo, bool withRoles)
    {
        _appDbClient.UpdateUser(id, userFullInfo);
        if (withRoles)
        {
            _appDbClient.UpdateUserRoles(id, userFullInfo.Roles);
        }
    }
}