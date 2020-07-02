using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Nancy.Diagnostics;
using Nancy.Json;
using Nancy.Json.Simple;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using vale_shorturl.Helpers;
using vale_shorturl.Model;

namespace vale_shorturl.DAL
{
    public class dalShortURL
    {
        private readonly IConfiguration _configuration;

        public dalShortURL(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public bool CreateShortURLRegister(string urlOriginal, string urlShort, string alias, int isAliasAutomatic)
        {
            string sql = string.Empty;
            dbAccess db = new dbAccess();

            sql += "INSERT INTO tblShortURL ";
            sql += "( ";
            sql += "OriginalURL ";
            sql += ", ShortURL ";
            sql += ", alias ";
            sql += ", isAutomaticAlias ";
            sql += ", dtCreation ";
            sql += ") ";
            sql += "VALUES ";
            sql += "( ";
            sql += "@OriginalURL ";
            sql += ", @ShortURL ";
            sql += ", @alias ";
            sql += ", @isAutomaticAlias ";
            sql += ", GETDATE() ";
            sql += "); ";

            sql += "SELECT 100 AS RETORNO; ";

            return db.TrataRetorno(db.SelectAccessDBSqlServer(sql, _configuration.GetConnectionString("DefaultConnection").ToString(),
            new SqlParameter("@OriginalURL", urlOriginal.Trim()),
                new SqlParameter("@ShortURL", urlShort.Trim()),
                new SqlParameter("@alias", alias),
                new SqlParameter("@isAutomaticAlias", isAliasAutomatic)));
        }

        public bool AliasExists(string alias)
        {
            string sql = string.Empty;
            dbAccess db = new dbAccess();

            sql += "SELECT COUNT(*) AS QTD ";
            sql += "FROM tblShortURL ";
            sql += "WHERE alias = @alias ";

            return db.TrataExists(db.SelectAccessDBSqlServer(sql, _configuration.GetConnectionString("DefaultConnection").ToString(),
                new SqlParameter("@alias", alias)));
        }

        public string GetURLByShort(string shortURL)
        {
            string sql = string.Empty;
            dbAccess db = new dbAccess();
            DataTable dt = new DataTable();

            sql += "SELECT OriginalURL ";
            sql += "FROM tblShortURL ";
            sql += "WHERE ShortURL = @shortURL ";

            dt = db.SelectAccessDBSqlServer(sql, _configuration.GetConnectionString("DefaultConnection").ToString(),
                new SqlParameter("@shortURL", shortURL.Trim().ToString()));

            if (dt.Rows.Count > 0)
            {
                return dt.Rows[0]["OriginalURL"].ToString().Trim();
            }
            else
            {
                return string.Empty;
            }
        }

        public bool RegistreShortURLAccess(string urlShort)
        {
            string sql = string.Empty;
            dbAccess db = new dbAccess();

            sql += "INSERT INTO tblShortURL_Access ";
            sql += "( ";
            sql += "ShortURL ";
            sql += ", dtAccess ";
            sql += ") ";
            sql += "VALUES ";
            sql += "( ";
            sql += "@ShortURL ";
            sql += ", GETDATE() ";
            sql += "); ";

            sql += "SELECT 100 AS RETORNO; ";

            return db.TrataRetorno(db.SelectAccessDBSqlServer(sql, _configuration.GetConnectionString("DefaultConnection").ToString(),
                new SqlParameter("@ShortURL", urlShort.Trim())));
        }

        public List<ResponseShortUrlViewModel_TopTen> GetTopTen()
        {
            string sql = string.Empty;
            dbAccess db = new dbAccess();
            DataTable dt = new DataTable();

            List<ResponseShortUrlViewModel_TopTen> tt = new List<ResponseShortUrlViewModel_TopTen>();

            sql += "SELECT Top 10 ShortURL, count(*) as qtdAccess ";
            sql += "FROM tblShortURL_Access ";
            sql += "group by ShortURL ";
            sql += "order by qtdAccess desc ";

            string ret = db.DataTableToJSON(db.SelectAccessDBSqlServer(sql, _configuration.GetConnectionString("DefaultConnection").ToString()));

            var o = JsonConvert.DeserializeObject<List<ResponseShortUrlViewModel_TopTen>>(ret);

            return o;
        }
    }
}

