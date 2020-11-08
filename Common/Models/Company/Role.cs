using Common.Constant;
using Common.Factory;
using Common.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models
{
    public class Role : DObject, IDObject
    {
        public bool IsChecked { get; set; }
    }

    public static class RoleRepository
    {
        public static List<Role> SelRoles(Role _param)
        {
            _param.Type = CommonConstant.TYPE_ROLE;
            return DaoFactory.GetList<Role>("Users.SelRole", _param);
        }
    }
}
