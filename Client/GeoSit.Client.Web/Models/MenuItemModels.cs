using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System;

namespace GeoSit.Client.Web.Models
{
    public class MenuItemModels
    {
        public long MenuItemId { get; set; }
        public string Nombre { get; set; }
        public List<MenuItemModels> SubMenuList { get; set; }
        public long? MenuItemIdPadre { get; set; }
        public long? Acceso { get; set; }
        //public string Controller { get; set; }
        public string Accion { get; set; }
        public string Icono { get; set; }
        public long? TipoAccion { get; set; }
        public long IdFuncion { get; set; }
        public void PrintPretty(string indent, bool last)
        {

            System.Diagnostics.Debug.Write(indent);
            if (last)
            {
                System.Diagnostics.Debug.Write("\\-");
                indent += "  ";
            }
            else
            {
                System.Diagnostics.Debug.Write("|-");
                indent += "| ";
            }
            System.Diagnostics.Debug.Write(Nombre + Environment.NewLine);

            if (SubMenuList != null)
            {
                for (int i = 0; i < SubMenuList.Count; i++)
                {
                    SubMenuList[i].PrintPretty(indent, i == SubMenuList.Count - 1);
                }
            }
        }
    }
}
