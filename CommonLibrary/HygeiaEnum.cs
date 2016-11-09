using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebService.CommonLibrary
{
    public class HygeiaEnum
    {

        public enum LogType : int
        {
            TraceLog = 1,
            InformationLog,
            ErrorLog
        }



        public enum DivisionType : int
        {
            Department = 1,
            Ward,
            Affiliation,
            Pharmacy
        }

        public enum JobType : int
        {
            Occupation = 1,
            Position
        }

        public enum ProfileType : int
        {
            Patient = 1,
            Admission,
            Nursing
        }

        public enum InOutType : int
        {
            Inpatient = 1,
            Outpatient,
            Both
        }

        public enum DiseaseType : int
        {
            Main = 1,
            Complication
        }


        public enum FlowsheetItemType : int
        {
            Observe = 1,
            Inspect
        }

        public enum MultiDisplayType : int
        {
            Combine = 1,
            Calculate
        }

        public enum MultiCalcType : int
        {
            Sum = 1,
            Max,
            Min,
            Avg
        }

        public enum SameDayDisplayType : int
        {
            Combine = 1,
            Calculate
        }

        public enum SameDayTimeZoneType : int
        {
            None = 1,
            TimeZone,
            Time
        }

        public enum SameDayCalcType : int
        {
            Max = 1,
            Min,
            Newest,
            Sum,
            Avg
        }

        public enum ObserveItemDataType : int
        {
            Integers = 1,
            Decimals,
            List,
            Time
        }



        public enum StampType : int
        {
            Folder = 0,
            OutpatientSearch,
            InpatientSearch,
            DocumentSearch,
            OrderSearch,
            Order,
            Text,
            Document,
            FlowsheetItem,
            CompoundStamp,
            OutcomeItem,
            TemporaryPath,
            FixednessPath,
            RecognitionPath
        }

        public enum StampBoxType : int
        {
            Karte = 1,
            Path
        }

        public enum StampCategoryType : int
        {
            Common = 1,
            Personal,
            Path
        }

        public enum StampDataType : int
        {
            Folder = 1,
            Stamp
        }



        public enum WorkSheetType : int
        {
            Section = 1,
            Inpatient,
            Movement,
            Document
        }



        public enum ProgressStatus : int
        {
            Doing = 1,
            Done
        }

        public enum SaveStatus : int
        {
            Temporary = 1,
            Fixedness,
            Recognition
        }



        public enum VarianceNoteStatus : int
        {
            Described = 1,
            Confirmed,
            Approved
        }



        public enum NumberingType : int
        {
            OrderNo = 1,
            DocumentNo,
            DiseaseNo,
            FlowsheetNo,
            FlowsheetItemNo,
            StampNo,
            WorkSheetNo,
            PathNo,
            VarianceNo,
            VarianceNoteNo,
            OutcomeItemNo,
            PathKey,
            PhysicalExamNo
        }

        public enum InformationType : int
        {
            Patient = 1,
            Disease,
            Order,
            Document
        }

        public enum OperationType : int
        {
            Created = 1,
            Modified,
            Removed,
            Browsed
        }

        public enum EditDataType : int
        {
            Patient = 1,
            Stamp,
            ClinicalPath
        }

        public enum EMRType : int
        {
            Karte = 1,
            Path
        }

        //2011-12-13朱兴骅  文书状态
        public enum NoteStatus : int
        {
            Temporary = 1,
            Publish

        }

        public enum OrderStatus : int
        {
            Development = 1,
            Publish,
            Arrangement,
            Enforcement,
            Suspension = 10,

            All = 11
            //显示所有医嘱状态
        }

        public enum PathStatus : int
        {
            Making = 1,
            Applay,
            Pause,
            Finish
        }

        public enum PathEndType : int
        {
            Normal = 1

        }

        public enum ProcessType : int
        {
            Develop = 1,
            Create
        }
    }
}