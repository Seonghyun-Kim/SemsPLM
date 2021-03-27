using Common.Constant;
using Common.Factory;
using Common.Interface;
using Common.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;

namespace Common.Models
{
    public class Person : DObject, IDObject
    {
        public string ID { get; set; }

        public string Password { get; set; }

        public string Email { get; set; }

        public string Rank { get; set; }

        public int? IsUse { get; set; }

        public DateTime? EnterDt { get; set; }

        public string Phone { get; set; }

        public int? DepartmentOID { get; set; }

        public string DepartmentNm { get; set; }

        public string ImgSign { get; set; }

    }

    public static class PersonRepository
    {
        public static int InsPerson(HttpSessionStateBase Context, Person _param)
        {
            List<Person> lPerson = PersonRepository.SelPersons(Context, new Person { ID = _param.ID });
            if (lPerson != null && lPerson.Count > 0)
            {
                return -1;
            }
            _param.Password = SemsSecureEDecode.Encrypt(_param.Password);
            int result = DaoFactory.SetInsert("Users.InsPerson", _param);
            return result;
        }

        public static int UtpPerson(Person _param)
        {
            return DaoFactory.SetUpdate("Users.UdtPerson", _param);
        }

        public static int UtpPwPerson(Person _param)
        {
            return DaoFactory.SetUpdate("Users.UdtPwPerson", _param);
        }

        public static int UtpIpPwPerson(Person _param)
        {
            return DaoFactory.SetUpdate("Users.UdtIpPwPerson", _param);
        }

        public static Person LoginSelPerson(Person _param)
        {
            _param.Type = CommonConstant.TYPE_PERSON;
            Person person = DaoFactory.GetData<Person>("Users.SelPerson", _param);
            if (person == null || person.DeleteDt != null)
            {
                person = null;
            }
            return person;
        }

        public static Person SelPerson(HttpSessionStateBase Context, Person _param)
        {
            _param.Type = CommonConstant.TYPE_PERSON;
            Person person = DaoFactory.GetData<Person>("Users.SelPerson", _param);
            person.Password = "**************";
            person.DepartmentNm = DObjectRepository.SelDObject(Context, new DObject { Type = CommonConstant.TYPE_DEPARTMENT, OID = person.DepartmentOID }).Name;
            if (person.DeleteDt != null)
            {
                person.Name = person.Name + "(삭제)";
            }
            return person;
        }

        public static List<Person> SelPersons(HttpSessionStateBase Context, Person _param)
        {
            _param.Type = CommonConstant.TYPE_PERSON;
            List<Person> lPerson = DaoFactory.GetList<Person>("Users.SelPerson", _param);
            List<DObject> SelDepartments = DObjectRepository.SelDObjects(Context, new DObject { Type = CommonConstant.TYPE_DEPARTMENT });
            lPerson.ForEach(person =>
            {
                person.Password = "**************";
                //person.DepartmentNm = DObjectRepository.SelDObject(Context, new DObject { Type = CommonConstant.TYPE_DEPARTMENT, OID = person.DepartmentOID }).Name;
                person.DepartmentNm = SelDepartments.Find(item => item.OID == person.DepartmentOID).Name;
                if (person.DeleteDt != null)
                {
                    person.Name = person.Name + "(삭제)";
                }
            });
            return lPerson;
        }

    }
}
