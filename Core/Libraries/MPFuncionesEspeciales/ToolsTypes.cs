using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Reflection;
using System.Globalization;
using System.Security;
using System.Drawing;

namespace MPFuncionesEspeciales
{
    public static class ToolsTypes
    {
        private static System.Globalization.NumberFormatInfo _numberInfo;
        public static System.Globalization.NumberFormatInfo NumberInfo
        {
            get
            {
                if (_numberInfo == null)
                {
                    _numberInfo = new System.Globalization.NumberFormatInfo();
                    _numberInfo.NumberDecimalSeparator = ".";
                    _numberInfo.NumberGroupSeparator = ",";
                }
                return _numberInfo;
            }

        }

        public static int ToInt32(string pValue)
        {
            int wRet = 0;
            int.TryParse(pValue, out wRet);
            return wRet;
        }

        public static int ToInt32(object objeto)
        {
            int wRet = 0;
            if (objeto != null && !DBNull.Value.Equals(objeto))
            {
                try
                {
                    int.TryParse(Convert.ToString(objeto), out wRet);
                }
                catch
                {
                    wRet = 0;
                }
            }
            return wRet;
        }

        public static int? ToInt32Nullable(object objeto)
        {
            int? wRet = null;
            if (objeto != null && !DBNull.Value.Equals(objeto))
            {
                try
                {
                    wRet = Convert.ToInt32(objeto);
                }
                catch
                {
                    wRet = null;
                }
            }
            return wRet;
        }

        public static long ToInt64(object objeto)
        {
            long wRet = 0;
            if (objeto != null && !DBNull.Value.Equals(objeto))
            {
                try
                {
                    wRet = Convert.ToInt64(objeto);
                }
                catch
                {
                    wRet = 0;
                }
            }
            return wRet;
        }

        public static float ToFloat(object objeto)
        {
            float wRet = 0;
            if (objeto != null && !DBNull.Value.Equals(objeto))
            {
                try
                {
                    string valor = objeto.ToString();
                    if (valor == string.Empty)
                    {
                        wRet = 0;
                    }
                    else
                    {
                        wRet = Convert.ToSingle(valor);
                    }
                }
                catch
                {
                    wRet = 0;
                }
            }
            return wRet;
        }

        public static float? ToFloatNullable(object objeto)
        {
            float? wRet = null;
            if (objeto != null && !DBNull.Value.Equals(objeto))
            {
                try
                {
                    wRet = Convert.ToSingle(objeto);
                }
                catch
                {
                    wRet = null;
                }
            }
            return wRet;
        }

        public static double ToDouble(object objeto)
        {
            double wRet = 0;
            if (objeto != null && !DBNull.Value.Equals(objeto))
            {
                try
                {
                    if (objeto is double)
                    {
                        wRet = Convert.ToDouble(objeto);
                    }
                    else
                    {
                        string valor = objeto.ToString();
                        if (valor == string.Empty)
                        {
                            wRet = 0;
                        }
                        else
                        {
                            wRet = Convert.ToDouble(valor);
                        }
                    }
                }
                catch
                {
                    wRet = 0;
                }
            }
            return wRet;
        }

        public static double ToDouble(object objeto, bool forceLocalization)
        {
            double wRet = 0;
            if (objeto != null && !DBNull.Value.Equals(objeto))
            {
                try
                {
                    string valor = objeto.ToString();
                    if (valor == string.Empty)
                    {
                        wRet = 0;
                    }
                    else
                    {
                        wRet = Convert.ToDouble(valor, ToolsTypes.NumberInfo);
                    }
                }
                catch
                {
                    wRet = 0;
                }
            }
            return wRet;
        }
        public static int DoubleToInt(double valor)
        {
            int result = 0;
            try
            {
                result = Convert.ToInt32(Math.Round(valor, 0).ToString());
            }
            catch { }
            return result;
        }
        public static decimal ToDecimal(object objeto)
        {
            decimal wRet = 0;
            if (objeto != null && !DBNull.Value.Equals(objeto))
            {
                string valor = objeto.ToString();
                decimal.TryParse(valor, out wRet);
            }
            return wRet;
        }
        public static string DecimalToString(decimal valor)
        {
            return valor.ToString().Replace(",", ".");
        }
        public static string DoubleToString(double valor)
        {
            return valor.ToString().Replace(",", ".");
        }
        public static string DoubleToString(string valor)
        {
            return valor.Replace(",", ".");
        }
        public static bool ToBoolean(string objeto)
        {
            if (!string.IsNullOrEmpty(objeto) && (objeto.ToLower() == "true" || objeto.ToLower() == "verdadero" || Math.Abs(ToInt32(objeto)) == 1))
            {
                return true;
            }
            return false;
        }
        public static bool ToBool(object obj)
        {
            int buffer;
            bool result = false;

            if (obj != null && !DBNull.Value.Equals(obj))
            {
                try
                {
                    result = (int.TryParse(obj.ToString(), out buffer) && buffer != 0);
                    if (!result)
                    {
                        result = Convert.ToBoolean(obj);
                    }
                }
                catch { }
            }
            return result;
        }
        public static DateTime DBReadDateTime(DataRow row, string campo)
        {
            DateTime valor = DateTime.MinValue;
            try
            {
                object objeto = row[campo];
                if (objeto != null && !DBNull.Value.Equals(objeto))
                {
                    valor = Convert.ToDateTime(objeto);
                }
            }
            catch { }
            return valor;
        }
        public static DateTime DBReadDateTime(string dateTime)
        {
            DateTime valor = DateTime.MinValue;
            if (!string.IsNullOrEmpty(dateTime))
            {
                try
                {
                    DateTime.TryParse(dateTime, out valor);
                }
                catch { }
            }
            return valor;
        }
        public static Nullable<DateTime> DBReadDateTimeNullable(DataRow row, string campo)
        {
            DateTime? valor = null;
            try
            {
                object objeto = row[campo];
                if (objeto != null && !DBNull.Value.Equals(objeto))
                {
                    valor = Convert.ToDateTime(objeto);
                }
            }
            catch { }
            return valor;
        }

        public static DateTime ToDateTime(object objeto, string culture)
        {
            return ToolsTypes.ToDateTime(objeto, culture, DateTime.MinValue);
        }

        public static DateTime ToDateTime(object objeto, string culture, DateTime defaultValue)
        {
            DateTime valor = defaultValue;
            if (string.IsNullOrEmpty(culture)) culture = "es-AR";
            if (objeto != null && !DBNull.Value.Equals(objeto))
            {
                try
                {
                    return Convert.ToDateTime(objeto, new CultureInfo(culture));
                }
                catch { }
            }
            return valor;
        }

        public static DateTime? ToNullableDateTime(object objeto)
        {
            DateTime? valor = null;
            if (objeto != null && !DBNull.Value.Equals(objeto))
            {
                valor = ToDateTime(objeto, string.Empty);
            }
            return valor;
        }

        public static string DBReadDateTimeToString(DataRow row, string campo)
        {
            string returnValue = string.Empty;
            try
            {
                object objeto = row[campo];
                if (objeto != null && !DBNull.Value.Equals(objeto))
                {
                    returnValue = Convert.ToDateTime(row[campo]).ToShortDateString();
                }
            }
            catch { }
            return returnValue;
        }

        public static string DateTimeToStringDBTime(DateTime? datetime)
        {
            string returnValue = string.Empty;
            if (datetime.HasValue)
            {
                returnValue = datetime.Value.Year.ToString("0000") + "-" + datetime.Value.Month.ToString("00") + "-" + datetime.Value.Day.ToString("00") + " " + datetime.Value.Hour.ToString("00") + ":" + datetime.Value.Minute.ToString("00") + ":" + datetime.Value.Second.ToString("00") + ".000";
            }
            return returnValue;
        }

        public static int GetHexToInt(string hexa)
        {
            return int.Parse(hexa, System.Globalization.NumberStyles.HexNumber);
        }

        public static long GetHexToLong(string hexa)
        {
            return long.Parse(hexa, System.Globalization.NumberStyles.HexNumber);
        }

        public static string GetHexToBin(string hexa)
        {
            try
            {
                int n = Convert.ToInt32(hexa, 16);
                string result = "";
                do
                {
                    result = ((n % 2 == 0) ? "0" : "1") + result;
                    n = n / 2;
                } while (n != 0);
                return result;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public static int BinToInt(string bin)
        {
            long l = Convert.ToInt64(bin, 2);
            int i = (int)l;
            return i;
        }

        public static string GetIntToBin(int n)
        {
            try
            {
                string result = "";
                do
                {
                    result = ((n % 2 == 0) ? "0" : "1") + result;
                    n = n / 2;
                } while (n != 0);
                return result;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public static string GetLongToBin(long n)
        {
            try
            {
                string result = "";
                do
                {
                    result = ((n % 2 == 0) ? "0" : "1") + result;
                    n = n / 2;
                } while (n != 0);
                return result;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public static string GetIntToHex(int valor)
        {
            return String.Format("{0:X}", valor).PadLeft(2, '0');
        }

        /// <summary>
        /// Crea una nueva instancia del objeto con la información original
        /// </summary>
        /// <param name="objOrigen">Objeto Origen</param>
        /// <param name="objDestino">Objeto Destino</param>
        /// <returns>true=clone ok, false=hubo algun error al clonar el objeto</returns>
        public static bool CloneSimple<T>(T objOrigen, out T objDestino)
        {
            try
            {
                objDestino = Activator.CreateInstance<T>();
                FieldInfo[] fields = objDestino.GetType().GetFields();

                int i = 0;
                foreach (FieldInfo fi in objOrigen.GetType().GetFields())
                {
                    fields[i].SetValue(objDestino, fi.GetValue(objOrigen));
                    i++;
                }
                return true;
            }
            catch (Exception)
            {
                objDestino = Activator.CreateInstance<T>();
                return false;
            }
        }

        public static string CompleteStringWithCharsAtLeft(string victim, int sizeToComplete, char charToComplete)
        {
            if (victim.Length < sizeToComplete)
            {
                while (victim.Length != sizeToComplete)
                {
                    victim = charToComplete + victim;
                }
            }
            return victim;
        }

        public static string StringReverse(string texto)
        {
            string s = string.Empty;
            for (int i = texto.Length - 2; i >= 0; i = i - 2)
            {
                s += texto.Substring(i, 2);
            }
            return s;
        }

        public static string StringReverseSimple(string texto)
        {
            char[] arr = texto.ToCharArray();
            Array.Reverse(arr);
            return new string(arr);
        }

        public static string GetEscapedString(string texto)
        {
            try
            {
                return SecurityElement.Escape(texto);
            }
            catch (Exception)
            {
                //ToolsLog.Write(ex, "GetEscapedString()");
                return texto;
            }
        }

        public static DateTime ConvertDateTime(DateTime fechaHora, TimeSpan UtcOffset)
        {
            try
            {
                return fechaHora.ToUniversalTime().Add(UtcOffset);
            }
            catch (Exception)
            {
                //ToolsLog.Write(ex, "ConvertTime()");
                return fechaHora;
            }
        }

        public static string ToString(object obj)
        {
            string result = string.Empty;
            if (obj != null && !DBNull.Value.Equals(obj))
            {
                try
                {
                    result = Convert.ToString(obj);
                }
                catch { }
            }
            return result;
        }

        public static string ToString(object obj, string nullText)
        {
            string result = nullText;
            if (obj != null && !DBNull.Value.Equals(obj))
            {
                try
                {
                    result = Convert.ToString(obj);
                }
                catch { }
            }
            return result;
        }

        public static void AddColumn(this DataTable table, string columnName, Type columnType)
        {
            if (!table.Columns.Contains(columnName))
            {
                table.Columns.Add(columnName, columnType);
            }
        }

        public static string ToHexValue(this Color color)
        {
            return "#" + color.R.ToString("X2") + color.G.ToString("X2") + color.B.ToString("X2");
        }


    }
}
