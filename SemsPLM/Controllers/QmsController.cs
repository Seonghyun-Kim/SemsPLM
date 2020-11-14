using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SemsPLM.Controllers
{
    public class QmsController : Controller
    {
        // GET: Qms
        public ActionResult Index()
        {
            return View();
        }

        #region -- 오픈이슈 
        #endregion

        #region -- 신속대응 등록 & 조회 
        #endregion

        #region -- 신속대응 일정관리 
        #endregion

        #region -- 봉쇄조치 
        #endregion

        #region -- 발생원인분석 
        #endregion

        #region -- 개선대책 
        #endregion

        #region -- Error Proof  
        #endregion

        #region -- LPA 부적합현황 
        #endregion

        #region -- LPA 대책서 
        #endregion

        #region -- 유효성 검증 
        public ActionResult CreateQuickValidation()
        {
            return View();
        }

        #endregion

        #region -- 표준화
        #endregion

        #region -- 교육
        public ActionResult CreateWorkerEducation()
        {
            return View();
        }
        #endregion
    }
}