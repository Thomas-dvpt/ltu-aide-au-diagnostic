using GlobalsOPCUAH;
using OpennessV16;
using Siemens.Engineering;
using Siemens.Engineering.Compiler;
using Siemens.Engineering.HW;
using Siemens.Engineering.HW.Features;
using Siemens.Engineering.SW;
using Siemens.Engineering.SW.Blocks;
using Siemens.Engineering.SW.Blocks.Interface;
using Siemens.Engineering.SW.ExternalSources;
using Siemens.Engineering.SW.Tags;
using Siemens.Engineering.SW.Types;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace TiaExplorePLCH
{
    /// <summary>
    /// Classe de traitement du projet Tia Portal du PLC 
    /// </summary>
    public class ExploreTiaPLCH
    {
        #region Variables

        private PLC_H_ProjectDefinitions m_oTiaProjectDefinitions;
        public PLC_H_ProjectDefinitions GetTiaProjectDefinitions() { return m_oTiaProjectDefinitions; }

        //Y3
        // Interface Openness sur Tia Portal
        public HMATIAOpenness_V16 m_oTiainterface = new HMATIAOpenness_V16();
        public HMATIAOpenness_V16 GetTiainterface() { return m_oTiainterface; }

        private string m_sError = string.Empty;

        // Indique si le projet Tia portal est déja sélectionné
        public bool m_bTiaPortalProjectIsSelected = false;
        public bool GetTiaPortalProjectIsSelected() { return m_bTiaPortalProjectIsSelected; }

        // Objet de traitement des fichiers Xml
        private XmlDocument m_oXmlDocument;

        //Donnees du fichiers excel
        private Dictionary<Tuple<int, int>, object> m_odata;

        //Infos pour le FC
        private List<string> m_odataCollection;

        #endregion


        /// <summary>
        /// Constructeur de la classe
        /// </summary>
        /// <param name="oTiaProjectDefinitions"></param>
        public ExploreTiaPLCH(PLC_H_ProjectDefinitions oTiaProjectDefinitions, Dictionary<Tuple<int, int>, object> odata, List<string> odataCollection)
        {
            m_oTiaProjectDefinitions = oTiaProjectDefinitions;
            m_oXmlDocument = new XmlDocument();
            //Excel
            m_odata = odata;
            //FC
            m_odataCollection = odataCollection;

        }

        /// <summary>
        /// Permet de choisir le projet Tia Portal de travail
        /// </summary>
        /// <param name="sError"> Code de l'erreur </param>
        /// <returns></returns>
        public bool ChooseTiaProject(ref string sError)
        {
            bool bRet = true;
            string sinfo = string.Empty;
            bool bCriticalError = false;

            sError = string.Empty;

            TiaPortalProjectSelection tiaselection = new TiaPortalProjectSelection();

            // Récupération de la liste des instances de projets Tia Portal en cours
            List<TiaPortalProcess> tiaportalcurrentprocess = HMATIAOpennessCurrentInstance.GetCurrentTiaPortalInstance();

            // Boucle de traitement de la liste des Tia Portal en cours de traitement
            foreach(TiaPortalProcess tiaprocess in tiaportalcurrentprocess)
            {
                try
                {
                    tiaselection.m_oDictionnaryTiaProcessList.Add(Path.GetFileNameWithoutExtension(tiaprocess.ProjectPath.Name), new HMATiaPortalProcess(tiaprocess));
                }
                catch
                {; }
            }

            // Affichage de la boite de dialogue
            tiaselection.ShowDialog();

            // On teste si un projet a été sélectionné ?
            if (tiaselection.bOneProjectSelected == true)
            {
                // on teste quel type de projet on a choisi ?
                switch(tiaselection.iTiaProjectSelectType)
                {
                    case TiaPortalProjectSelection.TiaProjectSelectionType.CurrentTiaProject:
                        if (m_oTiainterface.AttachTiaPortalInstance(tiaselection.GetSelectCurrentProject().GetTiaPortalProcess(), ref sinfo) == false)
                        {
                            sError = sinfo;
                            bRet = false;
                        }
                        // On associe le projet Tia Portal
                        m_oTiainterface.SetTIAPortalProject(tiaselection.GetSelectCurrentProject().GetTiaPortalProcess().ProjectPath.FullName);
                        if (m_oTiainterface.OpenCurrentTIAProjectFromInstance(ref sinfo, ref bCriticalError) == false)
                        {
                            sError = sinfo;
                            bRet = false;
                        }
                        break;

                    case TiaPortalProjectSelection.TiaProjectSelectionType.NewTiaProject:
                        m_oTiainterface = new HMATIAOpenness_V16(m_oTiaProjectDefinitions.bWithUITiaPortal, ref sinfo);
                        // Ouverture du projet avec le nouveau chemin
                        m_oTiainterface.SetTIAPortalProject(tiaselection.sNewTiaPortalSelectionPath);
                        if (m_oTiainterface.OpenTIAProject(m_oTiaProjectDefinitions.GetUserName(), m_oTiaProjectDefinitions.GetUncryptPasswordUser(), ref sinfo, ref bCriticalError) == false)
                        {
                            sError = sinfo;
                            bRet = false;
                        }
                        break;
                }

                m_bTiaPortalProjectIsSelected = true;

                //FIN SELECTION/OUVERTURE DE PROJET TIA
                

                bRet = true;
            }
            else
            {
                sError = @"No Tia project selected";
                bRet = false;
            }

            return bRet;

        }
            
        /// <summary>
        /// Permet de récupérer la liste des stations automates présents dans 
        /// </summary>
        /// <param name="sStationName"> Nom de la station sélectionnée </param>
        /// <param name="sError"> Texte de l'erreur </param>
        /// <returns></returns>
        public bool GetTiaStationFromProjectList(ref string sStationName, ref string sError)
        {
            bool bRet = true;
            List<Device> ListDevice = new List<Device>();

            while (true)
            {
                sStationName = string.Empty;
                // Lancement de l'énumération des stations dans le projet
                if (m_oTiainterface.EnumerationDevice(ref ListDevice, ref sError) == false)
                {
                    bRet = false;
                    break;
                }

                // Affichage de la boite de sélection d'une station du projet
                TiaPortalStationSelection tiaportalstationselection = new TiaPortalStationSelection();

                // Boucle de traitement de la liste des devices en cours de traitement
                foreach (Device device in ListDevice)
                {
                    tiaportalstationselection.m_oDictionnaryTiaStationList.Add(device.Name, new HMATiaPortalDevice(device));
                }

                // Affichage de la boite de dialogue
                tiaportalstationselection.ShowDialog();
                // Test si une station a été sélectionnée ?
                if (tiaportalstationselection.bOneStationSelected == true)
                {
                    sStationName = tiaportalstationselection.GetSelectCurrentStation().GetPortalDevice().Name;
                    bRet = true;
                }
                else
                {
                    bRet = false;
                    sError = @"No PLC station selected";
                }

                break;
            }

            return bRet;
        }

        /// <summary>
        /// Permet d'énumérer tous les folders, blocks, paramètres et tags 
        /// 
        /// </summary>
        /// <param name="tiaprojectforcpuH"> Configuration de la station H </param>
        /// <param name="sStationNameForPLCH"> Nom de la station correspondant au PLC </param>
        /// <param name="sErrorText"> Texte d'erreur de la fonction </param>
        /// <returns></returns>
        public bool EnumerateFoldersBlocksParametersAndTagsForOPCUA(ref TiaProjectForCPUH tiaprojectforcpuH, string sStationNameForPLCH, ref string sErrorText)
        {
            bool bRet = true;
            List<Device> ListDevice = new List<Device>();
            Device oPLCHStation = null;
            PlcSoftware oPlccontroller = null;
            //RAZ des donnees pour le FC aide au diag 
            m_odataCollection.Clear() ;

            sErrorText = string.Empty;

            while(true)
            {
                // Récupération de l'objet station correspondant à l'automate 
                // Lancement de l'énumération des stations dans le projet
                if (m_oTiainterface.EnumerationDevice(ref ListDevice, ref sErrorText) == false)
                {
                    bRet = false;
                    break;
                }
                // Recherche de la station 
                foreach(Device device in ListDevice)
                {
                    if (device.Name == sStationNameForPLCH)
                    {
                        oPLCHStation = device;
                        break;
                    }
                }
                // Test si la station H a été trouvée ?
                if (oPLCHStation == null)
                {
                    sErrorText = string.Format(@"Station '{0}' for PLC not found !", sStationNameForPLCH);
                    bRet = false;
                    break;
                }
                // Enumération de tous les blocs et paramètres avec le repère 
                if (EnumerateBlocksAndParameterOPCUAMarck(ref tiaprojectforcpuH, oPLCHStation, ref oPlccontroller, ref sErrorText) == false)
                {
                    bRet = false;
                    break;
                }
                // Enumération de tous les tags avec repère 
                if (EnumerateTagsWithOPCUAMarck(ref tiaprojectforcpuH, oPlccontroller, ref sErrorText) == false)
                {
                    bRet = false;
                    break;
                }

                // Enumération de tous les variables system de diagnostic de l'automate pour la remontée vers les passerelles
                if (EnumerateVariableSystemHTags(ref tiaprojectforcpuH, ref sErrorText) == false)
                {
                    bRet = false;
                    break;
                }

                break;
            }

            return bRet;
        }

        /// <summary>
        /// Permet d'énumérer tous les folders, blocs et paramètres avec le repère 
        /// </summary>
        /// <param name="sErrorText"></param>
        /// <returns></returns>
        bool EnumerateBlocksAndParameterOPCUAMarck(ref TiaProjectForCPUH tiaprojectforcpuH, Device oPLCHStation, ref PlcSoftware Plccontroller, ref string sErrorText)
        {
            bool bRet = true;
            PlcSoftware controllertarget = null;

            while (true)
            {
                // Récupération du controllertarget
                DeviceItem deviceItemToGetService = TryToFoundDeviceItemInDevice(oPLCHStation);
                if (deviceItemToGetService == null)
                {
                    sErrorText = @"Impossible to found PLC device item in device";
                    bRet = false;
                    break;
                }
                SoftwareContainer softwarecontainer = deviceItemToGetService.GetService<SoftwareContainer>() as SoftwareContainer;
                controllertarget = softwarecontainer.Software as PlcSoftware;
                if (controllertarget == null)
                {
                    sErrorText = @"controllertarget is null in this device";
                    bRet = false;
                    break;
                }

                // On affecte le PLC pour la fonction d'énumération des tags dans la méthode suivante
                Plccontroller = controllertarget;

                tiaprojectforcpuH.sPLCHControllerTargetName = controllertarget.Name;

                try
                {
                    // Recherche de tous les blocs présents dans la racine du projet CPU
                    SearchAllBlocInRootProgramFolder(controllertarget, ref tiaprojectforcpuH, tiaprojectforcpuH.GetRootNodefolderBlocks());
                    
                    
                    // Balayage de tous les folders de la racine et les sous folders et blocs associés
                    SearchAllFolderAndBlocInRootProgramFolder(controllertarget, ref tiaprojectforcpuH, tiaprojectforcpuH.GetRootNodefolderBlocks());
                }
                catch(Exception Ex)
                {
                    sErrorText = string.Format(@"Exception in EnumerateBlocksAndParameters() '{0}", Ex.Message);
                    bRet = false;
                    break;
                }

                break;
            }
            return bRet;
        }

        /// <summary>
        /// Recherche de l'item de type CPU dans le device
        /// </summary>
        /// <param name="oPLCHStation"></param>
        /// <returns></returns>
        public DeviceItem TryToFoundDeviceItemInDevice(Device oPLCHStation)
        {
            DeviceItem deviceitem = null;
            List<DeviceItem> oListDeviceItem = new List<DeviceItem>();
            string sError = string.Empty;
            SoftwareContainer softwarecontainer;

            if (m_oTiainterface.EnumerationDeviceItems(oPLCHStation, ref oListDeviceItem, ref sError) == true)
            {
                // Boucle de recherche de l'item de type CPU
                foreach(DeviceItem item in oListDeviceItem)
                {
                    softwarecontainer = item.GetService<SoftwareContainer>() as SoftwareContainer;
                    if (softwarecontainer != null)
                    {
                        deviceitem = item;
                        break;
                    }
                }
            }

            return deviceitem;
        }

        /// <summary>
        /// Permet de rechercher tous les blocs dans le folder en cours 
        /// </summary>
        /// <param name="controllertarget"></param>
        /// <param name="tiaprojectforcpuH"></param>
        void SearchAllBlocInRootProgramFolder(PlcSoftware controllertarget, ref TiaProjectForCPUH tiaprojectforcpuH, TreeNode nodesource)
        {
            List<PlcBlock> oListBlocks = new List<PlcBlock>();
            string sErreurText = string.Empty;
            string sHeaderFamilly = string.Empty;
            string sNewName = string.Empty;

            // Enumération de tous les blocs pour ce répertoire
            if (m_oTiainterface.EnumerateBlockUserProgramForPlc(controllertarget, ref oListBlocks, ref sErreurText) == true)
            {
                foreach (PlcBlock bloc in oListBlocks)
                {
                    // Test si on se trouve sur un bloc FC alors EXPORT
                    if (bloc is FC) //DataBlock
                    {
                        TiaPortalBloc tiabloc = new TiaPortalBloc(bloc.Name, nodesource, tiaprojectforcpuH.GetNextFolderVariableId(), sNewName);
                        // Enumération des liste des paramètres du bloc
                        ExportTiaBlockDBAndEnumerateOPCUAParameters(controllertarget, (bloc as FC), tiabloc, ref tiaprojectforcpuH);
                    }
                }
            }
        }

        /// <summary>
        /// Permet d'extraire le nouveau nom et l'identiant de la famille
        /// </summary>
        /// <param name="sHeaderFamilly"> Champ de référence complet </param>
        /// <param name="sFamillyBlockName"> Identifiant de la famille OPC UA </param>
        /// <param name="sNewName"> Identifiant du nouveau nom </param>
        bool ExtractNewNameAndFamillyBlock(string sHeaderFamilly, ref string sFamillyBlockName, ref string sNewName)
        {
            bool bRet = false;
            string[] oSplitList = new string[0];

            while (true)
            {
                sFamillyBlockName = string.Empty;
                sNewName = string.Empty;

                // Test si un identifiant de famille est spécifié ?
                if (sHeaderFamilly.ToUpper().Contains(m_oTiaProjectDefinitions.sFamilyBlocMarck.ToUpper()) == false)
                {
                    // Ce bloc n'est pas concerné pas la remontée OPC UA
                    bRet = false;
                    break;
                }
                // Test si un nom de remplacement est spécifié
                oSplitList = sHeaderFamilly.ToUpper().Split(m_oTiaProjectDefinitions.sFamillyStrResearchNewBlocName[0]);
                if ((oSplitList.Count() == 0 || oSplitList.Count() == 1))
                {
                    // Ce bloc n'admet pas de nouveau nom
                    sFamillyBlockName = sHeaderFamilly;
                    sNewName = string.Empty;
                    bRet = true;
                    break;
                }
                else if (oSplitList.Count() == 2)
                {
                    // Pas de spécificateur de nouveau nom
                    // On teste si le premier champ est bien le spécificateur de famille OPC UA
                    if (oSplitList[0].ToUpper() != m_oTiaProjectDefinitions.sFamilyBlocMarck.ToUpper())
                    {
                        // Ce bloc est mal identifié
                        bRet = false;
                        break;
                    }
                    sFamillyBlockName = oSplitList[0];
                    sNewName = oSplitList[1];
                    bRet = true;
                    break;
                }

                break;
            }

            return bRet;
        }

        /// <summary>
        /// Exportation d'un bloc au format XML
        /// </summary>
        /// <param name="bloc"> Bloc a exporter </param>
        /// <param name="sXMLFileExport"> Nom du fichier XML pour l'exportation </param>
        /// <param name="sError"> Texte erreur </param>
        /// <returns></returns>
        public bool ExportBlocToXml(PlcBlock bloc, string sXMLFileExport, ref string sError)
        {
            bool bRet = true;
            sError = string.Empty;

            while (true)
            {
                try
                {
                    FileInfo fileinfo = new FileInfo(sXMLFileExport);
                    bloc.Export(fileinfo, ExportOptions.WithDefaults);
                    //                    bloc.Export(sXMLFileExport, ExportOptions.None);
                    //                    bloc.Export(sXMLFileExport, ExportOptions.WithReadOnly);
                }
                catch (Exception e)
                {
                    sError = string.Format("Exception dans fonction ExportBlocToXml() '{0}'", e.Message);
                    bRet = false;
                }
                break;
            }

            return (bRet);
        }

        /// <summary>
        /// Permet d'insérer une source à partir d'un fichier externe
        /// </summary>
        /// <param name="externalsourcesystemFolder"></param>
        /// <param name="sSourceName"></param>
        /// <param name="sFilePathSource"></param>
        /// <param name="sError"></param>
        /// <returns></returns>
        public bool ImportSourceFileToSourceFolder(PlcExternalSourceSystemGroup externalsourcesystemFolder, string sSourceName, string sFilePathSource, ref string sError)
        {
            bool bRet = true;
            sError = string.Empty;

            while (true)
            {
                try
                {
                    externalsourcesystemFolder.ExternalSources.CreateFromFile(sSourceName, sFilePathSource);
                }
                catch (Exception e)
                {
                    sError = string.Format("Exception dans fonction ImportSourceFileToSourceFolder() '{0}'", e.Message);
                    bRet = false;
                }
                break;
            }
            return (bRet);
        }



        /// <summary>
        /// Permet d'exporter le bloc et de rechercher les paramètres
        /// pour implémentation 
        /// </summary>
        /// <param name="db"></param>
        /// <param name="tiabloc"></param>
        bool ExportTiaBlockDBAndEnumerateOPCUAParameters(PlcSoftware controllertarget, FC db, TiaPortalBloc tiabloc, ref TiaProjectForCPUH tiaprojectforcpuH)
        {
            bool bRet = true;
            string sErrorText = string.Empty;

            string sXmlDBFile = m_oTiaProjectDefinitions.sPathApplication + @"HMADBExport.Xml";

            while (true)
            {
                try
                {
                    // Test si le fichier existe pour l'effacer avant export
                    if (File.Exists(sXmlDBFile) == true) File.Delete(sXmlDBFile);
                    // Exportation du FC au format Xml
                    bRet = m_oTiainterface.ExportBlocToXml(db, sXmlDBFile, ref sErrorText);
                    if (bRet == false) break;

                    // Traitement du fichier xml d'export du FC
                    bRet = ParseTiaXmlDBFile(controllertarget, sXmlDBFile, tiabloc, ref tiaprojectforcpuH);

                }
                catch
                {
                    bRet = false;
                }

                break;
            }

            return bRet;
        }

        
        //1
        /// <summary>
        /// Permet de parser le fichier Xml d'un FC exporté
        /// </summary>
        /// <param name="sXmlFile"></param>
        /// <param name="tiabloc"></param>
        /// <returns></returns>
        bool ParseTiaXmlDBFile(PlcSoftware controllertarget, string sXmlFile, TiaPortalBloc tiabloc, ref TiaProjectForCPUH tiaprojectforcpuH)
        {
            bool bRet = true;
            string sMemberName = string.Empty;
            string sMemberType = string.Empty;
            string sCommentarParameter = string.Empty;
            string sRawType = string.Empty;
            string sReadOnly = string.Empty;
            bool bReadOnly = false;
            string sCurrentPath = string.Empty;

            while(true)
            {
                // Test si le fichier existe ?
                if (File.Exists(sXmlFile) == false)
                {
                    bRet = false;
                    break;
                }
                // Ouverture du fichier xml 
                m_oXmlDocument.Load(sXmlFile);

                // Recherche de la liste des noeuds de définition des paramètres du FC
                foreach(XmlNode nodedocument in m_oXmlDocument.ChildNodes)
                {
                    if (nodedocument.Name == @"Document")
                    {
                        foreach (XmlNode node in nodedocument.ChildNodes)
                        {
                            if (node.Name == @"SW.Blocks.FC")
                            {
                                foreach (XmlNode nodebloc in node.ChildNodes)
                                {

                                    if (nodebloc.Name == @"AttributeList")
                                    {
                                        foreach (XmlNode nodeAttributeList in nodebloc.ChildNodes)
                                        {
                                            if (nodeAttributeList.Name == @"Name")
                                            {
                                                //Nom du bloc FC dans lequel on se trouve
                                                m_odataCollection.Add("//" + nodeAttributeList.InnerText.ToString());
                                            }
                                        }
                                    }

                                    if (nodebloc.Name == @"ObjectList")
                                    {
                                        foreach (XmlNode nodeobjectlist in nodebloc.ChildNodes)
                                        {
                                            if (nodeobjectlist.Name == @"SW.Blocks.CompileUnit")
                                            {
                                                // Enumération de tous les membres du reseau
                                                EnumerateMembers(nodeobjectlist, controllertarget, tiaprojectforcpuH, tiabloc, ref sCurrentPath, true);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                break;
            }

            return bRet;
        }

        /// <summary>
        /// Pemet d'énumérer les différents membres correspondant à des paramètres
        /// </summary>
        /// <param name="oListOfNodeMembers"></param>
        void EnumerateMembers(XmlNode oListOfNodeMembers, PlcSoftware controllertarget, TiaProjectForCPUH tiaprojectforcpuH, TiaPortalBloc tiabloc, ref string sCurrentPath, bool bFirstLevel)
        {
            
            string sMemberName = string.Empty;
            string sMemberType = string.Empty;
            string sCommentarParameter = string.Empty;
            string sRawType = string.Empty;
            string sReadOnly = string.Empty;
            bool bReadOnly = false;
            string sPreviousCurrentPath = string.Empty;

            
            bool bIdentCon = false;

            var UId_VariableEntree = new List<string>();
            var VariableEntree = new List<string>();
            var PatteBlockLTU = new List<string>();
            var UId_PatteBlockLTU = new List<string>();

            var UId_BlockLTU = new List<string>();
            var NomBlockLTU = new List<string>();
            var NomDBi = new List<string>();


            //Une fois que l'on a trouver chaque reseau 
            //on procede a la recherche de nos variables dedans
            /******************************************************************************************/
            foreach (XmlNode nodeCompileUnit in oListOfNodeMembers.ChildNodes)
            {
                if (nodeCompileUnit.Name == @"AttributeList")
                {
                    foreach (XmlNode nodeAttributeList in nodeCompileUnit)
                    {
                        if (nodeAttributeList.Name == @"NetworkSource")
                        {
                            foreach (XmlNode nodeNetworkSource in nodeAttributeList)
                            {
                                if (nodeNetworkSource.Name == @"FlgNet")
                                {
                                    foreach (XmlNode nodeFlgNet in nodeNetworkSource)
                                    {
                                        //PARTIE DATA ET BLOCS
                                        if (nodeFlgNet.Name == @"Parts")
                                        {
                                            foreach (XmlNode nodeParts in nodeFlgNet)
                                            {
                                                //ON CHERCHE POUR LES NOMS DES VARIABLES EN ENTREE DU BLOC
                                                if (nodeParts.Name == @"Access")
                                                {
                                                    //Sauvegarde du Access Uid
                                                    UId_VariableEntree.Add(nodeParts.Attributes["UId"].Value.ToString());

                                                    foreach (XmlNode nodeAccess in nodeParts)
                                                    {
                                                       
                                                        //Si on a une VARIABLE de connecter
                                                        if (nodeAccess.Name == @"Symbol")
                                                        {
                                                            
                                                            bool firstComponent = true;

                                                            foreach (XmlNode nodeSymbol in nodeAccess)
                                                            {
                                                                if (firstComponent == true)
                                                                {
                                                                    if (nodeSymbol.Name == @"Component")
                                                                    {
                                                                        //Sauvegarde du COMPONENT NAME
                                                                        VariableEntree.Add(nodeSymbol.Attributes["Name"].Value.ToString());
                                                                        firstComponent = false;
                                                                    }

                                                                }
                                                            }     
                                                        }
                                                        //TEST SI ON A UNE VARIABLE

                                                        //Si on a une CONSTANTE de connecter
                                                        if (nodeAccess.Name == @"Constant")
                                                        {
                                                            foreach (XmlNode nodeConstant in nodeAccess)
                                                            {
                                                                if (nodeConstant.Name == @"ConstantType")
                                                                {
                                                                    //Sauvegarde du COMPONENT NAME
                                                                    VariableEntree.Add(nodeConstant.Name.ToString());
                                                                }
                                                                
                                                            }
                                                            
                                                        }
                                                    }
                                                }
                                                //SI ON CHERCHE POUR LE NOM DU BLOC APPELE ET DU DBi
                                                if (nodeParts.Name == @"Call")
                                                {
                                                    //Sauvegarde de l'UId du FC appelé
                                                    UId_BlockLTU.Add(nodeParts.Attributes["UId"].Value.ToString());

                                                    foreach (XmlNode nodeCall in nodeParts)
                                                    {
                                                        if (nodeCall.Name == @"CallInfo")
                                                        {
                                                            //Sauvegarde Nom du FC appelé
                                                            NomBlockLTU.Add(nodeCall.Attributes["Name"].Value.ToString());

                                                            foreach (XmlNode nodeCallInfo in nodeCall)
                                                            {
                                                                if (nodeCallInfo.Name == @"Instance")
                                                                {
                                                                    foreach (XmlNode nodeInstance in nodeCallInfo)
                                                                    {
                                                                        if (nodeInstance.Name == @"Component")
                                                                        {
                                                                            //Sauvegarde Nom du DBi utilisé
                                                                            NomDBi.Add(nodeInstance.Attributes["Name"].Value.ToString());
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        //PARTIE LIENS DES BLOCS
                                        if (nodeFlgNet.Name == @"Wires")
                                        {
                                            foreach (XmlNode nodeWires in nodeFlgNet)
                                            {
                                                //LISTING DES LIENS
                                                if (nodeWires.Name == @"Wire")
                                                {
                                                    foreach (XmlNode nodeWire in nodeWires)
                                                    {
                                                        if(bIdentCon == true)
                                                        { 
                                                            // Enumération de toutes les IN/OUT cablés
                                                            if (nodeWire.Name == @"NameCon")
                                                            {
                                                                //Sauvegarde du NAMECON Name
                                                                PatteBlockLTU.Add(nodeWire.Attributes["Name"].Value.ToString());
                                                                //Sauvegarde du NAMECON UId
                                                                UId_PatteBlockLTU.Add(nodeWire.Attributes["UId"].Value.ToString());
                                                                bIdentCon = false;

                                                            }
                                                        }
                                                        // Enumération de tous les IDs de connexions
                                                        if (nodeWire.Name == @"IdentCon")
                                                        {
                                                            bIdentCon = true;
                                                        }
                                                    }
                                                }

                                            }
                                        }
                                    }
                                }
                            }


                        }
                    }
                }
            }

            //TRAITEMENT+ ANALYSE
            //COMPARAISON AVEC LES DATAS

            //On verifie chaque LTU que l'on a detecter dans le reseau
            for (int a = 0; a < NomBlockLTU.Count; a++)
            {
                //Lecture des Donnees Excel
                foreach (KeyValuePair<Tuple<int, int>, object> keyValue in m_odata)
                {
                    //On scan la premiere colonne du fichier Excel
                    if (keyValue.Key.Item2 == 1)
                    {
                        //Si le nom du LTU export est présent dans le fichier Excel
                        if (NomBlockLTU[a] == keyValue.Value.ToString())
                        {
                            //Ecriture de la premiere colonne dans le DBi
                            m_odataCollection.Add("\"" + NomDBi[a] + "\"" + ".sHmiUdt.backJump[0] := '" + NomBlockLTU[a] + "';");
                            //Sauvegarde de l'indice de la ligne du LTU trouver
                            int indiceLigne = keyValue.Key.Item1;

                            //Lecture des Donnees de la ligne LTU
                            foreach (KeyValuePair<Tuple<int, int>, object> keyValueH in m_odata)
                            {
                                //On scan la ligne a l'horizontal
                                if ((keyValueH.Key.Item1 == indiceLigne) && (keyValueH.Key.Item2 > 1))
                                {
                                    //On verifie que la cellule n'est pas vide
                                    if (keyValueH.Value.ToString() != "")
                                    {
                                            int b = 0;

                                            if (PatteBlockLTU.Contains(keyValueH.Value.ToString()) )
                                            {

                                                
                                                b = PatteBlockLTU.IndexOf(keyValueH.Value.ToString());
                                                //On verifie que ca correspond bien a l'UId du block LTU appele
                                                if (UId_PatteBlockLTU[b] == UId_BlockLTU[a] && VariableEntree[b] != "ConstantType")
                                                {
                                                    //Ecriture de l'info 
                                                    m_odataCollection.Add("\"" + NomDBi[a] + "\"" + ".sHmiUdt.backJump[" + ((keyValueH.Key.Item2) - 1) + "] := '" + VariableEntree[b] + "';");
                                                    PatteBlockLTU[b] = "";
                                                }
                                                else
                                                {
                                                    //Ecriture de l'info vide
                                                    m_odataCollection.Add("\"" + NomDBi[a] + "\"" + ".sHmiUdt.backJump[" + ((keyValueH.Key.Item2) - 1) + "] := '';");
                                                }


                                        }
                                            else 
                                            {
                                                //Ecriture de l'info vide
                                                m_odataCollection.Add("\"" + NomDBi[a] + "\"" + ".sHmiUdt.backJump[" + ((keyValueH.Key.Item2) - 1) + "] := '';");
                                            }

                                    }
                                    else 
                                    {
                                        break;
                                    }
                                    
                                }
                            }
                        }
                    }
                }
            }

        }


        /// <summary>
        /// Pemet d'énumérer les différents membres correspondant à des paramètres sous la liste principale
        /// </summary>
        /// <param name="oListOfNodeMembers"></param>
        void EnumerateMembersExt(XmlNode oListOfNodeMembers, PlcSoftware controllertarget, TiaProjectForCPUH tiaprojectforcpuH, TiaPortalBloc tiabloc, ref string sCurrentPath)
        {
            string sMemberName = string.Empty;
            string sMemberType = string.Empty;
            string sCommentarParameter = string.Empty;
            string sRawType = string.Empty;
            string sReadOnly = string.Empty;
            bool bReadOnly = false;

            // Traitement de toutes les membres définis dans la liste
            foreach (XmlNode Member in oListOfNodeMembers.ChildNodes)
            {
                // Test si l'on se touve sur un noeud de type "Member" ?
                if (Member.Name == @"Member")
                {
                    sMemberName = Member.Attributes["Name"].Value.ToString();
                    sMemberType = Member.Attributes["Datatype"].Value.ToString();

                    //**************************************************
                    // On teste les différents types de membres
                    //**************************************************

                    // Test si membre de type "UDT" ?
                    if (sMemberType.Contains(@"""") == true)
                    {
                        if (sCurrentPath.Length == 0) sCurrentPath = @"""" + sMemberName + @"""";
                        else sCurrentPath = sCurrentPath + "." + @"""" + sMemberName + @"""";
                        // On se trouve dans le cas d'un type complexe
                        // On récupère dans ce cas le nom du type de bloc
                        string[] sBlocType = sMemberType.Split('"');

                        // Exportation de l'UDT au format XML
                        if (ExportTiaUDT(controllertarget, sBlocType[1]) == true)
                        {
                            string sParentParameterName = sMemberName;
                            // On lance le parse du fichier UDT
                            ParseTiaXmlUDTFile(controllertarget, tiabloc, ref sParentParameterName, ref tiaprojectforcpuH, sCurrentPath);
                        }
                    }
                    // Test si membre de type "Struct" ?
                    else if (sMemberType == @"Struct")
                    {
                        if (sCurrentPath.Length == 0) sCurrentPath = @"""" + sMemberName + @"""";
                        else sCurrentPath = sCurrentPath + "." + @"""" + sMemberName + @"""";
                        // Lancement de l'énumération des membres de la structure
                        EnumerateMembersExt(Member, controllertarget, tiaprojectforcpuH, tiabloc, ref sCurrentPath);
                    }
                    // Test si membre de type "Array" ?
                    else if (sMemberType.IndexOf(@"Array[") == 0)
                    {
                        if (sCurrentPath.Length == 0) sCurrentPath = sMemberName + ":" + @"""" + sMemberType + @"""";
                        else sCurrentPath = sCurrentPath + "." + sMemberName + ":" + @"""" + sMemberType + @"""";
                        // Lancement de l'énumération des membres du tableau
                        EnumerateMembersExt(Member, controllertarget, tiaprojectforcpuH, tiabloc, ref sCurrentPath);
                    }
                    // Test si type simple ou autre ?
                    else
                    {
                        // On se trouve dans le cas d'un type simple
                        foreach (XmlNode nodemember in Member.ChildNodes)
                        {
                            // Récupération de l'attribut lecture seule
                            if (nodemember.Name == @"AttributeList")
                            {
                                // Balayage de tous les attributs
                                foreach (XmlNode nodeattribut in nodemember.ChildNodes)
                                {
                                    // On test si on se trouve sur l'attribut "ExternalWritable"
                                    if (nodeattribut.Attributes[@"Name"].Value == @"ExternalWritable")
                                    {
                                        // On récupère la valeur de l'attribut readonly
                                        sReadOnly = nodeattribut.FirstChild.Value.ToString();
                                        if (sReadOnly == @"false") bReadOnly = true;
                                        else bReadOnly = false;
                                        break;
                                    }
                                }
                            }

                            // On recherche si un commentaire est défini dans le noeud pour ce paramètre
                            if (nodemember.Name == @"Comment")
                            {
                                foreach (XmlNode nodecomment in nodemember.ChildNodes)
                                {
                                    if (nodecomment.Name == @"MultiLanguageText")
                                    {
                                        // On récupère le commentaire pour ce paramètre
                                        sCommentarParameter = nodecomment.FirstChild.Value.ToString();
                                        // On teste si ce commentaire contient un attribut OPCUA
                                        if (sCommentarParameter.ToUpper().IndexOf(m_oTiaProjectDefinitions.sCommentarBlocParameterMarck.ToUpper()) == 0)
                                        {
                                            sRawType = sMemberType;
                                            // On supprime la longueur dans le cas d'une string
                                            if (sMemberType.Contains("String[") == true) sMemberType = @"String";
                                            // On ajoute ce paramètre à la liste des variables liées à notre bloc
                                            TiaPortalVariable tiaparameter = new TiaPortalVariable(sMemberName, tiabloc, sMemberType, sRawType, tiaprojectforcpuH.GetNextFolderVariableId(),
                                                                                                   null, sCommentarParameter, string.Empty, bReadOnly,
                                                                                                   sCurrentPath);
                                            tiabloc.VariablesList.Add(tiaparameter);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Permet d'exporter un type de variable en XML (UDT)
        /// </summary>
        /// <param name="sUDTName"> Nom de l'UDT qui est unique </param>
        /// <returns></returns>
        bool ExportTiaUDT(PlcSoftware controllertarget, string sUDTName)
        {
            bool bRet = true;
            string sErrorText = string.Empty;
            string sXmlUDTFile = m_oTiaProjectDefinitions.sPathApplication + @"HMAUDTExport.Xml";
            PlcType oUdt = null;

            while (true)
            {
                try
                {
                    // Test si le fichier existe pour l'effacer avant export
                    if (File.Exists(sXmlUDTFile) == true) File.Delete(sXmlUDTFile);
                    // Recherche de l'UDT dans le programme
                    oUdt = m_oTiainterface.GetUDTByName(controllertarget, sUDTName, ref sErrorText);
                    if (oUdt == null)
                    {
                        bRet = false;
                        break;
                    }
                    // EXportation du DB au format Xml
                    bRet = m_oTiainterface.ExportUDTToXml(oUdt, sXmlUDTFile, ref sErrorText);
                    if (bRet == false) break;
                }
                catch
                {
                    bRet = false;
                }

                break;
            }

            return bRet;
        }

        //2
        /// <summary>
        /// Permet de parser un type de données API
        /// </summary>
        /// <param name="sUDTName"> Nom de l'UDT </param>
        /// <param name="sParentParameterName"> Nom du paramètre parent </param>
        /// <param name="tiaparameters"> Liste des paramètres contenus dans ce paramètre </param>
        /// <returns></returns>
        bool ParseTiaXmlUDTFile(PlcSoftware controllertarget, TiaPortalBloc blocparent, ref string sParentParameterName, 
                                ref TiaProjectForCPUH tiaprojectforcpuH, string sCurrentPath)
        {
            bool bRet = true;
            string sMemberName = string.Empty;
            string sMemberType = string.Empty;
            string sCommentarParameter = string.Empty;
            string sXmlUDTFile = m_oTiaProjectDefinitions.sPathApplication + @"HMAUDTExport.Xml";
            string sRawType = string.Empty;
            string sReadOnly = string.Empty;
            bool bReadOnly = false;
            string sCurrentPathVariable = sCurrentPath;

            while (true)
            {
                // Test si le fichier existe ?
                if (File.Exists(sXmlUDTFile) == false)
                {
                    bRet = false;
                    break;
                }
                // Ouverture du fichier xml 
                m_oXmlDocument.Load(sXmlUDTFile);

                // Recherche de la liste des noeuds de définition des paramètres du FC
                foreach (XmlNode nodedocument in m_oXmlDocument.ChildNodes)
                {
                    if (nodedocument.Name == @"Document")
                    {
                        foreach (XmlNode node in nodedocument.ChildNodes)
                        {
                            if (node.Name == @"SW.Blocks.FC")
                            {
                                foreach (XmlNode nodebloc in node.ChildNodes)
                                {
                                    if (nodebloc.Name == @"ObjectList")
                                    {
                                        foreach (XmlNode nodeobjectlist in nodebloc.ChildNodes)
                                        {
                                            if (nodeobjectlist.Name == @"SW.Blocks.CompileUnit")
                                            {
                                                foreach (XmlNode nodeCompileUnit in nodeobjectlist.ChildNodes)
                                                {
                                                    if (nodeCompileUnit.Name == @"AttributeList")
                                                    {
                                                        foreach (XmlNode nodeAttributeList in nodeCompileUnit)
                                                        {
                                                            if (nodeAttributeList.Name == @"NetworkSource")
                                                            {
                                                                foreach (XmlNode nodeNetworkSource in nodeAttributeList)
                                                                {
                                                                    if (nodeNetworkSource.Name == @"FlgNet")
                                                                    {
                                                                        foreach (XmlNode nodeFlgNet in nodeNetworkSource)
                                                                        {
                                                                            //PARTIE DATA ET BLOCS
                                                                            if (nodeFlgNet.Name == @"Parts")
                                                                            {
                                                                                foreach (XmlNode nodeParts in nodeFlgNet)
                                                                                {
                                                                                    //SI ON CHERCHE POUR LE NOM DE LA VARIABLE EN ENTREE DU BLOC
                                                                                    if (nodeParts.Name == @"Access")
                                                                                    {
                                                                                        foreach (XmlNode nodeAccess in nodeParts)
                                                                                        {
                                                                                            if (nodeAccess.Name == @"Symbol")
                                                                                            {
                                                                                                foreach (XmlNode nodeSymbol in nodeAccess)
                                                                                                {
                                                                                                    if (nodeSymbol.Name == @"Component")
                                                                                                    {
                                                                                                        // Enumération de tous les membres de la section
                                                                                                        EnumerateMembers(nodeSymbol, controllertarget, tiaprojectforcpuH, blocparent, ref sCurrentPath, false);
                                                                                                        #region Modifs
                                                                                                        /*
                                                                                                        // Traitement de toutes les sections définis
                                                                                                        foreach (XmlNode Member in nodeSection.ChildNodes)
                                                                                                        {
                                                                                                            sMemberName = Member.Attributes["Name"].Value.ToString();
                                                                                                            sMemberType = Member.Attributes["Datatype"].Value.ToString();

                                                                                                            // Test quel est le type du membre pour prendre en compte les types de UDT
                                                                                                            if (sMemberType.Contains(@"""") == true)
                                                                                                            {
                                                                                                                // On se trouve dans le cas d'un type complexe
                                                                                                                // On récupère dans ce cas le nom du type de bloc
                                                                                                                string[] sBlocType = sMemberType.Split('"');

                                                                                                                // Exportation de l'UDT au format XML
                                                                                                                if (ExportTiaUDT(controllertarget, sBlocType[1]) == true)
                                                                                                                {
                                                                                                                    string sParentParameterName = sMemberName;
                                                                                                                    // On lance le parse du fichier UDT
                                                                                                                    ParseTiaXmlUDTFile(controllertarget, tiabloc, ref sParentParameterName, ref tiaprojectforcpuH);
                                                                                                                }
                                                                                                            }
                                                                                                            else
                                                                                                            {
                                                                                                                // On se trouve dans le cas d'un type simple
                                                                                                                foreach (XmlNode nodemember in Member.ChildNodes)
                                                                                                                {
                                                                                                                    // Récupération de l'attribut lecture seule
                                                                                                                    if (nodemember.Name == @"AttributeList")
                                                                                                                    {
                                                                                                                        // Balayage de tous les attributs
                                                                                                                        foreach (XmlNode nodeattribut in nodemember.ChildNodes)
                                                                                                                        {
                                                                                                                            // On test si on se trouve sur l'attribut "ExternalWritable"
                                                                                                                            if (nodeattribut.Attributes[@"Name"].Value == @"ExternalWritable")
                                                                                                                            {
                                                                                                                                // On récupère la valeur de l'attribut readonly
                                                                                                                                sReadOnly = nodeattribut.FirstChild.Value.ToString();
                                                                                                                                if (sReadOnly == @"false") bReadOnly = true;
                                                                                                                                else bReadOnly = false;
                                                                                                                                break;
                                                                                                                            }
                                                                                                                        }
                                                                                                                    }

                                                                                                                    // On recherche si un commentaire est défini dans le noeud pour ce paramètre
                                                                                                                    if (nodemember.Name == @"Comment")
                                                                                                                    {
                                                                                                                        foreach (XmlNode nodecomment in nodemember.ChildNodes)
                                                                                                                        {
                                                                                                                            if (nodecomment.Name == @"MultiLanguageText")
                                                                                                                            {
                                                                                                                                // On récupère le commentaire pour ce paramètre
                                                                                                                                sCommentarParameter = nodecomment.FirstChild.Value.ToString();
                                                                                                                                // On teste si ce commentaire contient un attribut OPCUA
                                                                                                                                if (sCommentarParameter.ToUpper().IndexOf(m_oTiaProjectDefinitions.sCommentarBlocParameterMarck.ToUpper()) == 0)
                                                                                                                                {
                                                                                                                                    sRawType = sMemberType;
                                                                                                                                    // On supprime la longueur dans le cas d'une string
                                                                                                                                    if (sMemberType.Contains("String[") == true) sMemberType = @"String";
                                                                                                                                    // On ajoute ce paramètre à la liste des variables liées à notre bloc
                                                                                                                                    TiaPortalVariable tiaparameter = new TiaPortalVariable(sMemberName, tiabloc, sMemberType, sRawType, tiaprojectforcpuH.GetNextFolderVariableId(), 
                                                                                                                                                                                            null, sCommentarParameter, string.Empty, bReadOnly);
                                                                                                                                    tiabloc.VariablesList.Add(tiaparameter);
                                                                                                                                }
                                                                                                                            }
                                                                                                                        }
                                                                                                                    }
                                                                                                                }
                                                                                                            }
                                                                                                        }
                                                                                                        */
                                                                                                        #endregion
                                                                                                    }
                                                                                                }
                                                                                            }
                                                                                        }
                                                                                    }
                                                                                    //SI ON CHERCHE POUR LE NOM DU BLOC APPELE ET DU DBi
                                                                                    if (nodeParts.Name == @"Call")
                                                                                    {
                                                                                        foreach (XmlNode nodeCall in nodeParts)
                                                                                        {
                                                                                            if (nodeCall.Name == @"CallInfo")
                                                                                            {
                                                                                                foreach (XmlNode nodeCallInfo in nodeCall)
                                                                                                {
                                                                                                    if (nodeCallInfo.Name == @"Instance")
                                                                                                    {
                                                                                                        foreach (XmlNode nodeInstance in nodeCallInfo)
                                                                                                        {
                                                                                                            if (nodeInstance.Name == @"Component")
                                                                                                            {
                                                                                                                // Enumération de tous les membres de la section
                                                                                                                EnumerateMembers(nodeInstance, controllertarget, tiaprojectforcpuH, blocparent, ref sCurrentPath, false);

                                                                                                            }
                                                                                                        }
                                                                                                    }
                                                                                                }
                                                                                            }
                                                                                        }
                                                                                    }
                                                                                }
                                                                            }
                                                                            //PARTIE LIENS DES BLOCS
                                                                            if (nodeFlgNet.Name == @"Wires")
                                                                            {
                                                                                foreach (XmlNode nodeWires in nodeFlgNet)
                                                                                {
                                                                                    //LISTING DES LIENS
                                                                                    if (nodeWires.Name == @"Wire")
                                                                                    {
                                                                                        foreach (XmlNode nodeWire in nodeWires)
                                                                                        {
                                                                                            // Enumération de tous les IDs de connexions
                                                                                            if (nodeWire.Name == @"OpenCon")
                                                                                            {
                                                                                                // Enumération de tous les IDs de connexions
                                                                                                EnumerateMembers(nodeWire, controllertarget, tiaprojectforcpuH, blocparent, ref sCurrentPath, false);
                                                                                            }
                                                                                            // Enumération de toutes les IN/OUT cablés
                                                                                            if (nodeWire.Name == @"NameCon")
                                                                                            {
                                                                                                // Enumération de toutes les IN/OUT cablés
                                                                                                EnumerateMembers(nodeWire, controllertarget, tiaprojectforcpuH, blocparent, ref sCurrentPath, false);
                                                                                            }
                                                                                        }
                                                                                    }

                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                }


                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                break;
            }

            return bRet;
        }

        /// <summary>
        /// Permet de rechercher tous les folders et blocs depuis la root
        /// </summary>
        /// <param name="controllertarget"></param>
        /// <param name="tiaprojectforcpuH"></param>
        void SearchAllFolderAndBlocInRootProgramFolder(PlcSoftware controllertarget, ref TiaProjectForCPUH tiaprojectforcpuH, TreeNode nodesource)
        {
            // Boucle de recherche de tous les floders
            foreach (PlcBlockUserGroup blockUserFolder in controllertarget.BlockGroup.Groups)
            {
                // Ajout du folder dans notre arbre
                TreeNode _node = new TreeNode(blockUserFolder.Name);
                _node.Tag = new TiaPortalFolder(blockUserFolder.Name,
                                                tiaprojectforcpuH.GetNextFolderVariableId());
                // V1.0.2
                if (nodesource != null) nodesource.Nodes.Add(_node);
                // Boucle de balayage des blocs s'ils existent
                
                EnumBlocksInFolderBlocks(controllertarget, blockUserFolder, ref tiaprojectforcpuH, _node);
                // Enumération de tous les sous folder
                EnumerateBlockUserFolders(controllertarget, blockUserFolder, ref tiaprojectforcpuH, _node);

            }
        }

        /// <summary>
        /// Enumération de tous les sous folder pour un folder
        /// </summary>
        /// <param name="blockUserFolder"> Répertoire source programme </param>
        /// <param name="treenodesource"> noeud parent </param>
        private void EnumerateBlockUserFolders(PlcSoftware controllertarget, PlcBlockUserGroup blockUserFolder, ref TiaProjectForCPUH tiaprojectforcpuH, TreeNode nodesource)
        {

            foreach (PlcBlockUserGroup subBlockUserFolder in blockUserFolder.Groups)
            {
                // Ajout du folder dans la liste
                TreeNode _node = new TreeNode(subBlockUserFolder.Name);
                _node.Tag = new TiaPortalFolder(subBlockUserFolder.Name,
                                                tiaprojectforcpuH.GetNextFolderVariableId());
                nodesource.Nodes.Add(_node);
                // Boucle de balayage des blocs s'ils existent
                EnumBlocksInFolderBlocks(controllertarget, subBlockUserFolder, ref tiaprojectforcpuH, _node);
                EnumerateBlockUserFolders(controllertarget, subBlockUserFolder, ref tiaprojectforcpuH, _node);
            }
        }

        /// <summary>
        /// Enumération de tous les blocs présents dans le folder spécifié
        /// </summary>
        /// <param name="Blocs"> Folder contenant les blocs programmes </param>
        /// <param name="treenodesource"> noeud parent </param>
        private void EnumBlocksInFolderBlocks(PlcSoftware controllertarget, PlcBlockGroup Blocs, ref TiaProjectForCPUH tiaprojectforcpuH, TreeNode nodesource)
        {
            string sNewName = string.Empty;
            string sHeaderFamilly = string.Empty;

            // Boucle de traitement de tous les blocs du répertoire
            foreach (PlcBlock bloc in Blocs.Blocks)
            {
                // Test si on se trouve sur un bloc FC
                //EXPORT
                if (bloc is FC)
                {
                    // On se touve sur un bloc FC
                    // On ajoute le bloc à la liste interne
                    TreeNode node = new TreeNode(bloc.Name);
                    TiaPortalBloc tiabloc = new TiaPortalBloc(bloc.Name, nodesource, tiaprojectforcpuH.GetNextFolderVariableId(), sNewName);
                    // Enumération des liste des paramètres du bloc
                    ExportTiaBlockDBAndEnumerateOPCUAParameters(controllertarget, (bloc as FC), tiabloc, ref tiaprojectforcpuH);
                    // Ajout du bloc dans la liste interne
                    tiaprojectforcpuH.oListblocs.Add(tiabloc);
                    node.Tag = tiabloc;
                    // On ajoute le noeud a notre arborescence interne
                    nodesource.Nodes.Add(node);

                }
            }
        }


        /// <summary>
        /// Permet d'énumérer tous les tags contenus dans les différentes table avec l'attributs OPC UA
        /// </summary>
        /// <param name="tiaprojectforcpuH"> objet global de définition des différentes variables </param>
        /// <param name="Plccontroller"> Référence au programme automate </param>
        /// <param name="sErrorText"> Texte de l'erreur renvoyé </param>
        /// <returns></returns>
        private bool EnumerateTagsWithOPCUAMarck(ref TiaProjectForCPUH tiaprojectforcpuH, PlcSoftware Plccontroller, ref string sErrorText)
        {
            bool bRet = true;

            sErrorText = string.Empty;

            while(true)
            {
                try
                {
                    // Recherche de toutes les tables présentes dans la racine
                    SearchAllVariableTableInRootPlcTagTableFolder(Plccontroller, ref tiaprojectforcpuH, tiaprojectforcpuH.GetRootNodefolderTags());
                    // Balayage de tous les folders de la racine et les sous folders et tables associées
                    SearchAllFolderAndTagTableInRootTagTableFolder(Plccontroller, ref tiaprojectforcpuH, tiaprojectforcpuH.GetRootNodefolderTags());
                }
                catch (Exception Ex)
                {
                    sErrorText = string.Format(@"Exception in EnumerateTagsWithOPCUAMarck() '{0}", Ex.Message);
                    bRet = false;
                    break;
                }

                break;
            }

            return bRet;
        }

        /// <summary>
        /// Permet de rechercher des tables de tags dans un folder
        /// </summary>
        /// <param name="controllertarget"></param>
        /// <param name="tiaprojectforcpuH"></param>
        /// <param name="nodesource"></param>
        void SearchAllVariableTableInRootPlcTagTableFolder(PlcSoftware controllertarget, ref TiaProjectForCPUH tiaprojectforcpuH, TreeNode nodesource)
        {
            PlcTagTableComposition tagTables = controllertarget.TagTableGroup.TagTables;

            // Boucle de traitement des tables présentes sous la racine
            foreach(PlcTagTable tagTable in tagTables)
            {
                // On énumère la liste des variables pour récupérer la liste avec l'attribut OPC UA
                ReadAllVariablesInTableWithOPCUAFlag(tagTable, ref tiaprojectforcpuH, nodesource);
            }
        }

        /// <summary>
        /// Permet de lire les variables dans la table avec l'attribut OPC UA
        /// </summary>
        /// <param name="tagTable"> Table de tags </param>
        /// <param name="tiaprojectforcpuH"> Objet reférence globale </param>
        /// <param name="nodesource"> Noeud parent dans l'arborescence </param>
        void ReadAllVariablesInTableWithOPCUAFlag(PlcTagTable tagTable, ref TiaProjectForCPUH tiaprojectforcpuH, TreeNode nodesource)
        {
            List<string> oListAttributs = new List<string>();
            oListAttributs.Add(@"ExternalWritable");
            IList<object> oListValueAttributes;
            bool bReadOnly = false;

            // Boucle de balayage de tous les tags de la table
            foreach (PlcTag tag in tagTable.Tags)
            {
                // On test si un commentaire existe sur ce tag ?
                if (tag.Comment.Items.Count > 0)
                {
                    // On teste alors si le flag OPC UA est présent ?
                    if (tag.Comment.Items[0].Text.ToUpper().IndexOf(m_oTiaProjectDefinitions.sCommentarTagVariableMarck.ToUpper()) == 0)
                    {
                        // Lecture de l'attribut de readonly
                        oListValueAttributes = tag.GetAttributes(oListAttributs);
                        if ((bool)oListValueAttributes[0] == false) bReadOnly = true;
                        else bReadOnly = false;
                        // Ajout de la variable à la liste
                        TiaPortalVariable tiaPortalVariable = new TiaPortalVariable(tag.Name, null, tag.DataTypeName, tag.DataTypeName, tiaprojectforcpuH.GetNextFolderVariableId(), 
                                                                                    nodesource, tag.Comment.Items[0].Text, string.Empty, bReadOnly, string.Empty);
                        tiaprojectforcpuH.oListTagsVariables.Add(tiaPortalVariable);
                    }
                }
            }
        }

        /// <summary>
        /// Permet de rechercher tous les folders et table de tags dans la racine
        /// </summary>
        /// <param name="controllertarget"></param>
        /// <param name="tiaprojectforcpuH"></param>
        /// <param name="nodesource"></param>
        void SearchAllFolderAndTagTableInRootTagTableFolder(PlcSoftware controllertarget, ref TiaProjectForCPUH tiaprojectforcpuH, TreeNode nodesource)
        {
            // Boucle de recherche de tous les floders
            foreach (PlcTagTableUserGroup controllerTagUserFolder in controllertarget.TagTableGroup.Groups)
            {
                // Ajout du folder dans notre arbre
                TreeNode _node = new TreeNode(controllerTagUserFolder.Name);
                _node.Tag = new TiaPortalFolder(controllerTagUserFolder.Name,
                                                tiaprojectforcpuH.GetNextFolderVariableId());
                // V1.0.2
                if (nodesource != null) nodesource.Nodes.Add(_node);
                // Boucle de balayage des tagtables si elles existent
                EnumTagTableInFolderTagTable(controllerTagUserFolder, ref tiaprojectforcpuH, _node);
                // Enumération de tous les sous folder dans le folder tagTable
                EnumerateTagTableUserGroup(controllerTagUserFolder, ref tiaprojectforcpuH, _node);
            }
        }

        /// <summary>
        /// Permet d'énumérer toutes les tagtable dans le folder tagtable
        /// </summary>
        /// <param name="controllerTagUserFolder"> Tagtable user folder</param>
        /// <param name="tiaprojectforcpuH"> Référence à l'objet global </param>
        /// <param name="nodesource"> Noeud parent </param>
        void EnumTagTableInFolderTagTable(PlcTagTableUserGroup controllerTagUserFolder, ref TiaProjectForCPUH tiaprojectforcpuH, TreeNode nodesource)
        {
            List<string> oListAttributs = new List<string>();
            oListAttributs.Add(@"ExternalWritable");
            IList<object> oListValueAttributes;
            bool bReadOnly = false;

            // Boucle de traitement de toutes les tagtables du répertoire
            foreach (PlcTagTable tagTable in controllerTagUserFolder.TagTables)
            {
                // Boucle de balayage de tous les tags de la table
                foreach (PlcTag tag in tagTable.Tags)
                {
                    // On test si un commentaire existe sur ce tag ?
                    if (tag.Comment.Items.Count > 0)
                    {
                        // On teste alors si le flag OPC UA est présent ?
                        if (tag.Comment.Items[0].Text.ToUpper().IndexOf(m_oTiaProjectDefinitions.sCommentarTagVariableMarck.ToUpper()) == 0)
                        {
                            // Lecture de l'attribut de readonly
                            oListValueAttributes = tag.GetAttributes(oListAttributs);
                            if ((bool)oListValueAttributes[0] == false) bReadOnly = true;
                            else bReadOnly = false;

                            // Ajout de la variable à la liste
                            TiaPortalVariable tiaPortalVariable = new TiaPortalVariable(tag.Name, null, tag.DataTypeName, tag.DataTypeName, tiaprojectforcpuH.GetNextFolderVariableId(), 
                                                                                        nodesource, tag.Comment.Items[0].Text, string.Empty, bReadOnly, string.Empty);
                            tiaprojectforcpuH.oListTagsVariables.Add(tiaPortalVariable);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Permet d'énumérer toutes les folders tagtable dans le folder tagtable
        /// </summary>
        /// <param name="controllerTagUserFolder"> Tagtable user folder</param>
        /// <param name="tiaprojectforcpuH"> Référence à l'objet global </param>
        /// <param name="nodesource"> Noeud parent </param>
        void EnumerateTagTableUserGroup(PlcTagTableUserGroup controllerTagUserFolder, ref TiaProjectForCPUH tiaprojectforcpuH, TreeNode nodesource)
        {
            foreach (PlcTagTableUserGroup TagUserFolder in controllerTagUserFolder.Groups)
            {
                // Ajout du folder dans la liste
                TreeNode _node = new TreeNode(TagUserFolder.Name);
                _node.Tag = new TiaPortalFolder(TagUserFolder.Name,
                                                tiaprojectforcpuH.GetNextFolderVariableId());
                nodesource.Nodes.Add(_node);
                // Boucle de balayage des tagtables si elles existent
                EnumTagTableInFolderTagTable(TagUserFolder, ref tiaprojectforcpuH, _node);
                EnumerateTagTableUserGroup(TagUserFolder, ref tiaprojectforcpuH, _node);
            }
        }

        /// <summary>
        /// Permet d'énumérer toutes les variables system pour le diagnsotic H
        /// </summary>
        /// <param name="tiaprojectforcpuH"></param>
        /// <param name="sErrorText"></param>
        /// <returns></returns>
        private bool EnumerateVariableSystemHTags(ref TiaProjectForCPUH tiaprojectforcpuH, ref string sErrorText)
        {
            bool bRet = true;

            sErrorText = string.Empty;

            while (true)
            {
                try
                {
                    // Insertion de toutes les variables systemes pour le diagnostic H
                    ScanAllSystemHVariables(ref tiaprojectforcpuH, tiaprojectforcpuH.GetRootNodefolderHSystemTags());
                }
                catch (Exception Ex)
                {
                    sErrorText = string.Format(@"Exception in EnumerateVariableSystemHTags() '{0}", Ex.Message);
                    bRet = false;
                    break;
                }

                break;
            }

            return bRet;
        }

        /// <summary>
        /// Permet d'ajouter le répertoire des variables system et les variables system H
        /// </summary>
        /// <param name="tiaprojectforcpuH"></param>
        /// <param name="nodesource"></param>
        void ScanAllSystemHVariables(ref TiaProjectForCPUH tiaprojectforcpuH, TreeNode nodesource)
        {
            string sVariableMappingName = string.Empty;
            TiaPortalVariable tiaPortalVariable = null;

            // Ajout du folder correspondant au répertoire des variables system H
            TreeNode _node = new TreeNode(tiaprojectforcpuH.m_sRootVariableSystemHName);
            _node.Tag = new TiaPortalFolder(tiaprojectforcpuH.m_sRootVariableSystemHName,
                                            tiaprojectforcpuH.GetNextFolderVariableId());
            nodesource.Nodes.Add(_node);

            // Ajout de la liste des variables system H
            // Ajout de "ServiceLevel"
            sVariableMappingName = @"""OPC_UA_Server_State"".""iServiceLevel""";
//            tiaPortalVariable = new TiaPortalVariable(@"ServiceLevel", null, @"Int16", @"Int16", tiaprojectforcpuH.GetNextFolderVariableId(),
//                                                      _node, @"Service level de la redondance OPC UA", sVariableMappingName, true, string.Empty);
            // Modification passage en lecture écriture pour gestion Stop des cpus gateway
            tiaPortalVariable = new TiaPortalVariable(@"ServiceLevel", null, @"Int16", @"Int16", tiaprojectforcpuH.GetNextFolderVariableId(),
                                                      _node, @"Service level de la redondance OPC UA", sVariableMappingName, false, string.Empty);

            tiaprojectforcpuH.oListSystemHTagsVariables.Add(tiaPortalVariable);

            // Ajout de "EnableWriteToPLCH"
            sVariableMappingName = @"""OPC_UA_Server_State"".""bEnableWriteToPLCH""";
            tiaPortalVariable = new TiaPortalVariable(@"EnableWriteToPLCH", null, @"Boolean", @"Boolean", tiaprojectforcpuH.GetNextFolderVariableId(),
                                                      _node, @"Valide l'écriture dans le PLC H distant", sVariableMappingName, false, string.Empty);
            tiaprojectforcpuH.oListSystemHTagsVariables.Add(tiaPortalVariable);

            // Ajout de "ServerStatus"
            sVariableMappingName = @"""OPC_UA_Server_State"".""iServerStatus""";
            tiaPortalVariable = new TiaPortalVariable(@"ServerStatus", null, @"Int16", @"Int16", tiaprojectforcpuH.GetNextFolderVariableId(),
                                                      _node, @"Status du serveur OPC au niveau du PLC H distant", sVariableMappingName, true, string.Empty);
            tiaprojectforcpuH.oListSystemHTagsVariables.Add(tiaPortalVariable);

            // Ajout de "PLC_H_Redundant_State"
            sVariableMappingName = @"""OPC_UA_Server_State"".""iPLC_H_Redundant_State""";
            tiaPortalVariable = new TiaPortalVariable(@"PLC_H_Redundant_State", null, @"Int16", @"Int16", tiaprojectforcpuH.GetNextFolderVariableId(),
                                                      _node, @"Etat de la redondance du PLC_H", sVariableMappingName, true, string.Empty);
            tiaprojectforcpuH.oListSystemHTagsVariables.Add(tiaPortalVariable);

            // Ajout de "PLc_CPU_1_State"
            sVariableMappingName = @"""OPC_UA_Server_State"".""iPLc_CPU_1_State""";
            tiaPortalVariable = new TiaPortalVariable(@"PLc_CPU_1_State", null, @"Int16", @"Int16", tiaprojectforcpuH.GetNextFolderVariableId(),
                                                      _node, @"Etat de la CPU 1 du PLC H", sVariableMappingName, true, string.Empty);
            tiaprojectforcpuH.oListSystemHTagsVariables.Add(tiaPortalVariable);

            // Ajout de "PLc_CPU_2_State"
            sVariableMappingName = @"""OPC_UA_Server_State"".""iPLc_CPU_2_State""";
            tiaPortalVariable = new TiaPortalVariable(@"PLc_CPU_2_State", null, @"Int16", @"Int16", tiaprojectforcpuH.GetNextFolderVariableId(),
                                                      _node, @"Etat de la CPU 2 du PLC H", sVariableMappingName, true, string.Empty);
            tiaprojectforcpuH.oListSystemHTagsVariables.Add(tiaPortalVariable);
        }


    }


    /// <summary>
    /// Classe de représentaation d'une interface process Tia Portal
    /// </summary>
    public class HMATiaPortalProcess
    {
        #region Variables

        private TiaPortalProcess m_oTiaPortalProcess;
        public TiaPortalProcess GetTiaPortalProcess() { return m_oTiaPortalProcess; }

        #endregion

        /// <summary>
        /// Constructeur par defaut de la classe
        /// </summary>
        /// <param name="oTiaPortalProcess"></param>
        public HMATiaPortalProcess(TiaPortalProcess oTiaPortalProcess)
        {
            m_oTiaPortalProcess = oTiaPortalProcess;
        }

        // Pour ajout dans la combobox de sélection des Process Tia Portal
        public override string ToString()
        {
            return string.Format("{0}", Path.GetFileNameWithoutExtension(m_oTiaPortalProcess.ProjectPath.FullName));
        }
    }

    /// <summary>
    /// Classe de représentation d'un device dans un projet TIA Portal
    /// </summary>
    public class HMATiaPortalDevice
    {
        #region Variables

        private Device m_oTiaPortalDevice;
        public Device GetPortalDevice() { return m_oTiaPortalDevice; }

        #endregion

        /// <summary>
        /// Constructeur par defaut de la classe
        /// </summary>
        /// <param name="oTiaPortalProcess"></param>
        public HMATiaPortalDevice(Device device)
        {
            m_oTiaPortalDevice = device;
        }

        // Pour ajout dans la combobox de sélection des devices Tia Portal
        public override string ToString()
        {
            return string.Format("{0}", m_oTiaPortalDevice.Name);
        }
    }

}
