using Common.Factory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models
{
    public class BMenu
    {
        public int? OID { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public int? Ord { get; set; }

        public int? IsUse { get; set; }

        public List<BSubMenu> SubMenu { get; set; }
    }

    public static class BMenuRepository
    {
        public static List<BMenu> SelBMenu()
        {
            List<BMenu> lBMenu = DaoFactory.GetList<BMenu>("Menu.SelBMenu", null);
            lBMenu.ForEach(bMenu =>
            {
                 bMenu.SubMenu = DaoFactory.GetList<BSubMenu>("Menu.SelBSubMenu", new BSubMenu { MenuOID = bMenu.OID });
            });
            return lBMenu;
        }
    }
}
