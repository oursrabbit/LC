using Microsoft.AspNetCore.Mvc;

namespace LCAPI.Models
{
    /// <summary>
    /// StatusCode不是200的时候，返回这个JSON结构描述错误信息
    /// </summary>
    public class RestResultJSON
    {
        /// <summary>
        /// Status Code
        /// </summary>
        public string code { get; set; }

        /// <summary>
        /// Error Message
        /// </summary>
        public string message { get; set; }

        /// <summary>
        /// Useless
        /// </summary>
        public Object data { get; set; }

        public static string CreateRestResultJSONString(string code, string message, object data)
        {
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(
                 new RestResultJSON()
                 {
                     code = code,
                     message = message,
                     data = data
                 });
            return json ?? "";
        }

        public static ActionResult CreateRestJSONResult(string code, string message, object data, int statusCode)
        {
            var json = CreateRestResultJSONString(code, message, data);
            return new JsonResult(json) { StatusCode = statusCode };
        }
    }

    /// <summary>
    /// 全文检索时使用的数据结构
    /// </summary>
    public class FullTextSearchJSON
    {
        /// <summary>
        /// 如果为空字符串数组，表示返回所有数据
        /// 空字符串数组：不是null，是List.Count = 0
        /// 具有多个值时，使用且关系进行查询
        /// </summary>
        public List<string> searchtext { get; set; }
    }

    /// <summary>
    /// 属性查找
    /// 多个属性、多个属性值时，使用且关系进行查询
    /// </summary>
    public class SearchJSON
    {
        /// <summary>
        /// 该资源被上传至本系统中的时间，请使用ISO UTC+0 Date 字符串表示
        /// 
        /// 2023-07-19T16:00:00.000+00:00
        /// 
        /// 2023-07-19T16:00:00.000Z
        /// 
        /// 以上两个例子为UTC+0，填写时需要转换
        /// 
        /// 后端系统使用UTC+8保存数据，数据库中使用UTC+0保存数据
        /// </summary>
        public string resource_publish_date_start { get; set; }

        /// <summary>
        /// 该资源被上传至本系统中的时间，请使用ISO UTC+0 Date 字符串表示
        /// 
        /// 2023-07-19T16:00:00.000+00:00
        /// 
        /// 2023-07-19T16:00:00.000Z
        /// 
        /// 以上两个例子为UTC+0，填写时需要转换
        /// 
        /// 后端系统使用UTC+8保存数据，数据库中使用UTC+0保存数据
        /// </summary>
        public string resource_publish_date_end { get; set; }

        /// <summary>
        /// 关键字，空数组表示忽略此项，List.Count == 0
        /// </summary>
        public List<String> resource_keyword { get; set; }

        /// <summary>
        /// 语种，空数组表示忽略此项，List.Count == 0
        /// </summary>
        public List<String> resource_lang { get; set; }

        /// <summary>
        /// 领域，空数组表示忽略此项，List.Count == 0
        /// </summary>
        public List<String> resource_tag { get; set; }

        /// <summary>
        /// 描述，空数组表示忽略此项，List.Count == 0
        /// </summary>
        public List<String> resource_description { get; set; }

        /// <summary>
        /// 同时查询原始文件名和中文文件名，或关系，空数组表示忽略此项，List.Count == 0
        /// </summary>
        public List<String> resource_file_name { get; set; }

        /// <summary>
        /// 装备分类，空数组表示忽略此项
        /// </summary>
        public List<string> video_equipment_tag { get; set; }
    }

    /// <summary>
    /// 属性查找
    /// 多个属性、多个属性值时，使用且关系进行查询
    /// </summary>
    public class SearchModelJSON
    {
        /// <summary>
        /// 该资源被上传至本系统中的时间，请使用ISO UTC+0 Date 字符串表示
        /// 
        /// 2023-07-19T16:00:00.000+00:00
        /// 
        /// 2023-07-19T16:00:00.000Z
        /// 
        /// 以上两个例子为UTC+0，填写时需要转换
        /// 
        /// 后端系统使用UTC+8保存数据，数据库中使用UTC+0保存数据
        /// </summary>
        public string resource_publish_date_start { get; set; }

        /// <summary>
        /// 该资源被上传至本系统中的时间，请使用ISO UTC+0 Date 字符串表示
        /// 
        /// 2023-07-19T16:00:00.000+00:00
        /// 
        /// 2023-07-19T16:00:00.000Z
        /// 
        /// 以上两个例子为UTC+0，填写时需要转换
        /// 
        /// 后端系统使用UTC+8保存数据，数据库中使用UTC+0保存数据
        /// </summary>
        public string resource_publish_date_end { get; set; }

        /// <summary>
        /// 关键字，空数组表示忽略此项，List.Count == 0
        /// </summary>
        public List<String> resource_keyword { get; set; }

        /// <summary>
        /// 模型类型，空数组表示忽略此项，List.Count == 0
        /// </summary>
        public List<String> resource_tag { get; set; }

        /// <summary>
        /// 描述，空数组表示忽略此项，List.Count == 0
        /// </summary>
        public List<String> resource_description { get; set; }

        /// <summary>
        /// 同时查询原始文件名和中文文件名，或关系，空数组表示忽略此项，List.Count == 0
        /// </summary>
        public List<String> resource_file_name { get; set; }

        /// <summary>
        /// 是否包含贴图，0：不包含，1：包含，其他值：忽略该项
        /// </summary>
        public int model_has_texture { get; set; }

        /// <summary>
        /// 模型备注，空数组表示忽略此项，List.Count == 0
        /// </summary>
        public List<String> model_notes { get; set; } 

    }

    /// <summary>
    /// 属性查找
    /// 多个属性、多个属性值时，使用且关系进行查询
    /// </summary>
    public class SearchAeJSON
    {
        /// <summary>
        /// 该资源被上传至本系统中的时间，请使用ISO UTC+0 Date 字符串表示
        /// 
        /// 2023-07-19T16:00:00.000+00:00
        /// 
        /// 2023-07-19T16:00:00.000Z
        /// 
        /// 以上两个例子为UTC+0，填写时需要转换
        /// 
        /// 后端系统使用UTC+8保存数据，数据库中使用UTC+0保存数据
        /// </summary>
        public string resource_publish_date_start { get; set; }

        /// <summary>
        /// 该资源被上传至本系统中的时间，请使用ISO UTC+0 Date 字符串表示
        /// 
        /// 2023-07-19T16:00:00.000+00:00
        /// 
        /// 2023-07-19T16:00:00.000Z
        /// 
        /// 以上两个例子为UTC+0，填写时需要转换
        /// 
        /// 后端系统使用UTC+8保存数据，数据库中使用UTC+0保存数据
        /// </summary>
        public string resource_publish_date_end { get; set; }

        /// <summary>
        /// 关键字，空数组表示忽略此项，List.Count == 0
        /// </summary>
        public List<String> resource_keyword { get; set; }

        /// <summary>
        /// 模型类型，空数组表示忽略此项，List.Count == 0
        /// </summary>
        public List<String> resource_tag { get; set; }

        /// <summary>
        /// 描述，空数组表示忽略此项，List.Count == 0
        /// </summary>
        public List<String> resource_description { get; set; }

        /// <summary>
        /// 同时查询原始文件名和中文文件名，或关系，空数组表示忽略此项，List.Count == 0
        /// </summary>
        public List<String> resource_file_name { get; set; }

        /// <summary>
        /// AE标题，空数组表示忽略此项，List.Count == 0
        /// </summary>
        public List<String> ae_title { get; set; }

    }
}
