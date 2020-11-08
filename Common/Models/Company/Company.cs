using Common.Constant;
using Common.Factory;
using Common.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models
{
    public class Company : DObject, IDObject
    {
    }

    public static class CompanyRepository
    {
        
        public static JqTreeModel SelOrganization()
        {
            DObject dCompany = DObjectRepository.SelDObject(new DObject { Type = CommonConstant.TYPE_COMPANY });
            JqTreeModel jqTreeModel = new JqTreeModel();
            jqTreeModel.id = dCompany.OID;
            jqTreeModel.label = dCompany.Name;
            jqTreeModel.icon = CommonConstant.ICON_COMPANY;
            jqTreeModel.iconsize = CommonConstant.DEFAULT_ICONSIZE;
            jqTreeModel.expanded = true;
            jqTreeModel.type = CommonConstant.TYPE_COMPANY;
            List<JqTreeModel> items = new List<JqTreeModel>();
            DObject tmpDepartment = null;
            DRelationshipRepository.SelRelationship(new DRelationship { Type = CommonConstant.RELATIONSHIP_DEPARTMENT, FromOID = dCompany.OID }).ForEach(item =>
            {
                if (tmpDepartment != null)
                {
                    tmpDepartment = null;
                }
                JqTreeModel innerJqTreeModel = new JqTreeModel();
                tmpDepartment = DObjectRepository.SelDObject(new DObject { Type = CommonConstant.TYPE_DEPARTMENT, OID = item.ToOID });
                innerJqTreeModel.id = tmpDepartment.OID;
                innerJqTreeModel.label = tmpDepartment.Name;
                innerJqTreeModel.icon = CommonConstant.ICON_DEPARTMENT;
                innerJqTreeModel.iconsize = CommonConstant.DEFAULT_ICONSIZE;
                innerJqTreeModel.expanded = true;
                innerJqTreeModel.type = CommonConstant.TYPE_DEPARTMENT;
                SelDepartment(innerJqTreeModel, tmpDepartment);
                items.Add(innerJqTreeModel);
            });
            jqTreeModel.items = items;

            return jqTreeModel;
        }

        public static void SelDepartment(JqTreeModel _jqxTree, DObject _param)
        {
            List<JqTreeModel> items = new List<JqTreeModel>();
            DObject tmpDepartment = null;
            DRelationshipRepository.SelRelationship(new DRelationship { Type = CommonConstant.RELATIONSHIP_DEPARTMENT, FromOID = _param.OID }).ForEach(item =>
            {
                if (tmpDepartment != null)
                {
                    tmpDepartment = null;
                }
                JqTreeModel innerJqTreeModel = new JqTreeModel();
                tmpDepartment = DObjectRepository.SelDObject(new DObject { Type = CommonConstant.TYPE_DEPARTMENT, OID = item.ToOID });
                innerJqTreeModel.id = tmpDepartment.OID;
                innerJqTreeModel.label = tmpDepartment.Name;
                innerJqTreeModel.icon = CommonConstant.ICON_DEPARTMENT;
                innerJqTreeModel.iconsize = CommonConstant.DEFAULT_ICONSIZE;
                innerJqTreeModel.expanded = true;
                innerJqTreeModel.type = CommonConstant.TYPE_DEPARTMENT;
                SelDepartment(innerJqTreeModel, tmpDepartment);
                items.Add(innerJqTreeModel);
            });
            _jqxTree.items = items;
        }

        public static JqTreeModel SelOrganizationWithPerson(List<string> checkitemtypes)
        {
            DObject dCompany = DObjectRepository.SelDObject(new DObject { Type = CommonConstant.TYPE_COMPANY });
            JqTreeModel jqTreeModel = new JqTreeModel();
            jqTreeModel.id = dCompany.OID;
            jqTreeModel.label = dCompany.Name;
            jqTreeModel.icon = CommonConstant.ICON_COMPANY;
            jqTreeModel.iconsize = CommonConstant.DEFAULT_ICONSIZE;
            jqTreeModel.expanded = true;
            jqTreeModel.type = CommonConstant.TYPE_COMPANY;
            jqTreeModel.checkitemtypes = checkitemtypes;
            List<JqTreeModel> items = new List<JqTreeModel>();
            DObject tmpDepartment = null;
            DRelationshipRepository.SelRelationship(new DRelationship { Type = CommonConstant.RELATIONSHIP_DEPARTMENT, FromOID = dCompany.OID }).ForEach(item =>
            {
                if (tmpDepartment != null)
                {
                    tmpDepartment = null;
                }
                JqTreeModel innerJqTreeModel = new JqTreeModel();
                tmpDepartment = DObjectRepository.SelDObject(new DObject { Type = CommonConstant.TYPE_DEPARTMENT, OID = item.ToOID });
                innerJqTreeModel.id = tmpDepartment.OID;
                innerJqTreeModel.label = tmpDepartment.Name;
                innerJqTreeModel.icon = CommonConstant.ICON_DEPARTMENT;
                innerJqTreeModel.iconsize = CommonConstant.DEFAULT_ICONSIZE;
                innerJqTreeModel.expanded = true;
                innerJqTreeModel.type = CommonConstant.TYPE_DEPARTMENT;
                innerJqTreeModel.checkitemtypes = checkitemtypes;
                SelDepartmentWithPerson(innerJqTreeModel, tmpDepartment, checkitemtypes);
                items.Add(innerJqTreeModel);
            });
            jqTreeModel.items = items;

            return jqTreeModel;
        }

        public static void SelDepartmentWithPerson(JqTreeModel _jqxTree, DObject _param, List<string> _checkitemtypes)
        {
            List<JqTreeModel> items = new List<JqTreeModel>();
            DObject tmpDepartment = null;
            DRelationshipRepository.SelRelationship(new DRelationship { Type = CommonConstant.RELATIONSHIP_DEPARTMENT, FromOID = _param.OID }).ForEach(item =>
            {
                if (tmpDepartment != null)
                {
                    tmpDepartment = null;
                }
                JqTreeModel innerJqTreeModel = new JqTreeModel();
                tmpDepartment = DObjectRepository.SelDObject(new DObject { Type = CommonConstant.TYPE_DEPARTMENT, OID = item.ToOID });
                innerJqTreeModel.id = tmpDepartment.OID;
                innerJqTreeModel.label = tmpDepartment.Name;
                innerJqTreeModel.icon = CommonConstant.ICON_DEPARTMENT;
                innerJqTreeModel.iconsize = CommonConstant.DEFAULT_ICONSIZE;
                innerJqTreeModel.expanded = true;
                innerJqTreeModel.type = CommonConstant.TYPE_DEPARTMENT;
                innerJqTreeModel.checkitemtypes = _checkitemtypes;
                List<Person> personItems = PersonRepository.SelPersons(new Person { DepartmentOID = tmpDepartment.OID });
                personItems.ForEach(personItem =>
                {
                    JqTreeModel personJqTreeModel = new JqTreeModel();
                    personJqTreeModel.id = personItem.OID;
                    personJqTreeModel.label = personItem.Name;
                    personJqTreeModel.icon = CommonConstant.ICON_PERSON;
                    personJqTreeModel.iconsize = CommonConstant.DEFAULT_ICONSIZE;
                    personJqTreeModel.type = CommonConstant.TYPE_PERSON;
                    personJqTreeModel.checkitemtypes = _checkitemtypes;
                    personJqTreeModel.value = personItem.DepartmentNm;
                    if (innerJqTreeModel.items == null)
                    {
                        innerJqTreeModel.items = new List<JqTreeModel>();
                    }
                    innerJqTreeModel.items.Add(personJqTreeModel);
                });
                SelDepartmentWithPerson(innerJqTreeModel, tmpDepartment, _checkitemtypes);
                items.Add(innerJqTreeModel);
            });
            if (_jqxTree.items == null)
            {
                _jqxTree.items = new List<JqTreeModel>();
            }
            _jqxTree.items.AddRange(items);
        }
    }
}
