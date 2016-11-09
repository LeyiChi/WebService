using System;
using System.Data;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

public class JSONHelper
{
    /// <summary>
    /// object转JSON格式
    /// </summary>
    /// <param name="_obj">对象</param>
    /// <returns></returns>
    public static string ObjectToJson(object _obj)
    {
        string strResult = string.Empty;

        try
        {
            if (_obj != null)
            {
                strResult = JavaScriptConvert.SerializeObject(_obj);
            }
        }
        catch (Exception ex)
        {
            return ex.Message.ToString();
        }

        return strResult;
    }

    /// <summary>
    /// DataTable转JSON格式
    /// </summary>
    /// <param name="_dt">DataTable</param>
    /// <returns></returns>
    public static string DataTableToJson(DataTable _dt)
    {
        string strResult = string.Empty;

        try
        {
            if ((_dt != null) && (_dt.Rows.Count > 0))
            {
                StringWriter writer = new StringWriter();
                new DataTableConverter().WriteJson(new JsonWriter(writer), _dt);
                strResult = writer.ToString();
            }
        }
        catch (Exception ex)
        {
            return ex.Message.ToString();
        }

        return strResult;
    }

    /// <summary>
    /// DatasSet转JSON格式
    /// </summary>
    /// <param name="_ds">DataSet</param>
    /// <returns></returns>
    public static string DataSetToJson(DataSet _ds)
    {
        string strResult = string.Empty;

        try
        {
            if ((_ds != null) && (_ds.Tables.Count > 0))
            {
                StringWriter writer = new StringWriter();
                new DataSetConverter().WriteJson(new JsonWriter(writer), _ds);
                strResult = writer.ToString();
            }
        }
        catch (Exception ex)
        {
            return ex.Message.ToString();
        }

        return strResult;
    }
}
