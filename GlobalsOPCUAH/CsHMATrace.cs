using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace GlobalsOPCUAH
{
    /// <summary>
    /// 
    /// Créateur : Siemens DF PD PA CS France
    /// Version : V1.0
    /// 
    /// Modificatons :
    /// 
    /// Cette classe permet de gérer les traces vers le debugger externe de Windows
    /// </summary>
    
    public class CsHMATrace
    {
        private string m_sApplicationName = "";
        public enum leveltype {level1, level2, level3};
        private bool m_bTraceIsActive = false;
        
        /// <summary>
        /// Constructor de la classe en spécifiant le nom de l'application concernée par 
        /// la trace
        /// </summary>
        /// <param name="sApplicationName"> Nom de l'application concernée par la trace</param>
        /// <param name="bTraceActive"> true : trace active ; false : trace inactive</param>
        public CsHMATrace(string sApplicationName, bool bTraceActive) { m_sApplicationName = sApplicationName; m_bTraceIsActive = bTraceActive;}
        
        /// <summary>
        /// Permet d'envoyer la trace vers le debugger externe en fonction d'un filtre
        /// </summary>
        /// <param name="sTrace"></param>
        /// <param name="level"></param>
        public void HMATrace(string sTrace, leveltype level)
        {
            string sFormatTrace = "";

            // Test si la trace est active ?
            if (m_bTraceIsActive == true)
            {
                sFormatTrace = string.Format("[{0}_{1}]:{2}", m_sApplicationName, (int)(level), sTrace);    
                System.Diagnostics.Trace.AutoFlush = true;
                System.Diagnostics.Trace.WriteLine(sFormatTrace);
            }
        }

        public bool GetTraceActive() { return (m_bTraceIsActive); }
        public void SetTraceActive(bool bTraceIsActive) { m_bTraceIsActive = bTraceIsActive; }

    }
}
