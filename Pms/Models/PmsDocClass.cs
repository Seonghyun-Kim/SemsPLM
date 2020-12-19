using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Common.Factory;
using Common.Interface;
using Common.Models;

namespace Pms.Models
{
    public class PmsDocClass : DObject, IDObject
    {
        public int? RootOID { get; set; }
        public int? FromOID { get; set; }
        public int? ToOID { get; set; }
        public string ProjectNm { get; set; }
        public string DocClassNm { get; set; }
        public string TaskNm { get; set; }


        public string RelOID                {get;set;}
        public string RelType               {get;set;}
        public string Children              {get;set;}
        public string expanded              {get;set;}
        public string Files                 {get;set;}
        public string DocToOID              {get;set;}
        public string DocClassOID           {get;set;}
        public string DocClassPID           {get;set;}
        public string DocClassPIDNm         {get;set;}
        public string DocName               {get;set;}
        public string DocOID                {get;set;}
        public string DocNo                 {get;set;}
        public string DocNoNm               {get;set;}
        public string DocSt                 {get;set;}
        public string DocStNm               {get;set;}
        public string DocRev                {get;set;}
        public string DatabaseFl            {get;set;}
        public string LinkOID               {get;set;}
        public string ViewUrl               {get;set;}
        public string EditUrl               {get;set;}
        public string PMSViewUrl            {get;set;}
        public string PMSEditUrl            {get;set;}
        public string UseFl                 {get;set;}

    }

    public static class PmsDocClassRepository
    {

    }
}
