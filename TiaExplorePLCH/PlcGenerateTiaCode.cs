using GlobalsOPCUAH;
using Siemens.Engineering.Compiler;
using Siemens.Engineering.HW;
using Siemens.Engineering.HW.Features;
using Siemens.Engineering.SW;
using Siemens.Engineering.SW.Blocks;
using Siemens.Engineering.SW.ExternalSources;
using Siemens.Engineering.SW.OpcUa;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TiaExplorePLCH
{
    /// <summary>
    /// Classe de génération du code automate pour le projet
    /// </summary>
    public class PlcGenerateTiaCode
    {
        #region Constantes
        
        const string sRootOPCUAFolder = @"AIDE_AU_DIAGNOSTIC";
        const string sOPCUAMappingFolder = @"FC_Aide_au_diagnostic";

        const string sRootEnableWriteTransmitTag = @"bNewValue_";

        const string sDBName_Global_Mapping_OPC_UA = @"Global_Mapping_OPC_UA";
        const string sDBTitle_Global_Mapping_OPC_UA = @"Mapping data for OPC UA Exchange PLC H";

        const string sDBName_Global_Mapping_OPC_UA_1 = @"Global_Mapping_OPC_UA_1";
        const string sDBTitle_Global_Mapping_OPC_UA_1 = @"Mapping data for OPC UA Exchange Gateway 1";
        const string sDBName_Global_Mapping_OPC_UA_2 = @"Global_Mapping_OPC_UA_2";
        const string sDBTitle_Global_Mapping_OPC_UA_2 = @"Mapping data for OPC UA Exchange Gateway 2";

        const string sDBName_Mapping_OPC_Server = @"Mapping_OPC_Server";
        const string sDBTitle_Mapping_OPC_Server = @"Mapping mémoire pour le serveur OPC Ua de la CPU";

        const string sDBName_Mapping_OPC_UA_From_PLC_H = @"Mapping_OPC_UA_From_PLC_H";
        const string sDBTitle_Mapping_OPC_UA_From_PLC_H = @"Mapping memory from OPC UA Gateway";

        const string sDBName_Mapping_OPC_UA_From_PLC_H_Previous = @"Mapping_OPC_UA_From_PLC_H_Previous";
        const string sDBTitle_Mapping_OPC_UA_From_PLC_H_Previous = @"Mapping memory from OPC UA Gateway";

        const string sDBName_Mapping_OPC_UA_To_PLC_H = @"Mapping_OPC_UA_To_PLC_H";
        const string sDBTitle_Mapping_OPC_UA_To_PLC_H = @"Mapping memory from OPC UA Gateway";

        const string sDBName_Mapping_OPC_Server_OB_Begin = @"Mapping_OPC_Server_OB_Begin";
        const string sDBTitle_Mapping_OPC_Server_OB_Begin = @"Mapping memory of start input cyclic OB";

        const string sDBName_Mapping_OPC_Server_OB_End = @"Mapping_OPC_Server_OB_End";
        const string sDBTitle_Mapping_OPC_Server_OB_End = @"Mapping memory of end cyclic OB";

        const string sFCName_Receive_From_PLC_H = @"Receive_From_PLC_H";
        const string sFCTitle_Receive_From_PLC_H = @"Update memory map from PLC Gateway";

        const string sFCName_Send_To_PLC_H = @"Send_To_PLC_H";
        const string sFCTitle_Send_To_PLC_H = @"Update memory map to PLC Gateway";

        const string sFCName_Fill_Mapping_OB_Begin = @"Fill_Mapping_OB_Begin";
        const string sFCTitle_Fill_Mapping_OB_Begin = @"Fill mapping memory start input cyclic OB";

        const string sFCName_Fill_Mapping_OB_End = @"Fill_Mapping_OB_End";
        const string sFCTitle_Fill_Mapping_OB_End = @"Fill mapping memory end cyclic OB";

        const string sOPCUAServerInterfaceName = @"Mapping_Interface_Plc_Gateway";

        const string sDBName_Mapping_OPC_UA_From_Gateway_1 = @"Mapping_OPC_UA_From_Gateway_1";
        const string sDBTitle_Mapping_OPC_UA_From_Gateway_1 = @"Mapping memory from OPC UA Gateway 1";
        const string sDBName_Mapping_OPC_UA_From_Gateway_2 = @"Mapping_OPC_UA_From_Gateway_2";
        const string sDBTitle_Mapping_OPC_UA_From_Gateway_2 = @"Mapping memory from OPC UA Gateway 2";


        const string sDBName_Mapping_OPC_UA_From_Gateway_Previous_1 = @"Mapping_OPC_UA_From_Gateway_Previous_1";
        const string sDBTitle_Mapping_OPC_UA_From_Gateway_Previous_1 = @"Mapping memory from OPC UA Gateway 1";
        const string sDBName_Mapping_OPC_UA_From_Gateway_Previous_2 = @"Mapping_OPC_UA_From_Gateway_Previous_2";
        const string sDBTitle_Mapping_OPC_UA_From_Gateway_Previous_2 = @"Mapping memory from OPC UA Gateway 2";

        const string sDBName_Mapping_OPC_UA_To_Gateway_1 = @"Mapping_OPC_UA_To_Gateway_1";
        const string sDBTitle_Mapping_OPC_UA_To_Gateway_1 = @"Mapping memory from OPC UA Gateway 1";
        const string sDBName_Mapping_OPC_UA_To_Gateway_2 = @"Mapping_OPC_UA_To_Gateway_2";
        const string sDBTitle_Mapping_OPC_UA_To_Gateway_2 = @"Mapping memory from OPC UA Gateway 2";

        const string sFCName_Receive_From_Gateway_1 = @"FC_Aide_au_diagnostic";
        const string sFCTitle_Receive_From_Gateway_1 = @"Update memory map from PLC Gateway 1";
        const string sFCName_Receive_From_Gateway_2 = @"Receive_From_Gateway_2";
        const string sFCTitle_Receive_From_Gateway_2 = @"Update memory map from PLC Gateway 2";

        const string sFCName_Send_To_Gateway_1 = @"Send_To_Gateway_1";
        const string sFCTitle_Send_To_Gateway_1 = @"Update memory map to PLC Gateway 1";
        const string sFCName_Send_To_Gateway_2 = @"Send_To_Gateway_2";
        const string sFCTitle_Send_To_Gateway_2 = @"Update memory map to PLC Gateway 2";


        #endregion


        #region Variables

        // Référence su l'objet d'exploration du projet TIA Portal H
        ExploreTiaPLCH m_oExploreTiaPLCH;

        // Référence sur les informations programme du S7-1500H
        private TiaProjectForCPUH m_oTiaProjectForCPUH;
        public TiaProjectForCPUH GetTiaProjectForCPUH() { return m_oTiaProjectForCPUH; }
        public void SetTiaProjectForCPUH(TiaProjectForCPUH oTiaProjectForCPUH) { m_oTiaProjectForCPUH = oTiaProjectForCPUH; }

        private string m_sError = string.Empty;

        //Infos pour le FC
        private List<string> m_odataCollection;

        #endregion


        /// <summary>
        /// Constructeur par défaut de la classe
        /// </summary>
        /// <param name="oExploreTiaPLCH"></param>
        public PlcGenerateTiaCode(ExploreTiaPLCH oExploreTiaPLCH, List<string> odataCollection)
        {
            m_oExploreTiaPLCH = oExploreTiaPLCH;
            //FC
            m_odataCollection = odataCollection;
        }

        /// <summary>
        /// Permet 
        /// </summary>
        /// <param name="sInput"> Chaine d'entrée </param>
        /// <param name="iNumber"> nombre d'espace </param>
        /// <returns></returns>
        string AddSpaceBeginStr(string sInput, int iNumber)
        {
            string sRet = " ";
            for (int iCar = 1; iCar < iNumber; iCar++) sRet = sRet + " ";
            sRet = sRet + sInput;

            return sRet;
        }

        /// <summary>
        /// Permet de compiler un plc avec vérification des erreurs
        /// </summary>
        /// <param name="sStationName"></param>
        /// <returns></returns>
        public bool CompileThisPlcAndCheckErrors(string sStationName, ref string sErrorText)
        {
            bool bRet = true;
            sErrorText = string.Empty;
            PlcSoftware oThisPlc = null;
            Device ostationdevice = null;
            DeviceItem ostationdeviceitem = null;
            bool bOneErrorCompilation = false;

            while (true)
            {
                // Recherche de la référence de la station passée en paramètre
                oThisPlc = GetThisStationByName(sStationName, ref ostationdevice, ref ostationdeviceitem, ref sErrorText);
                if (oThisPlc == null)
                {
                    sErrorText = string.Format(@"Station with name {0} not found in TIA Project", sStationName);
                    bRet = false;
                    break;
                }

                // Compilation complète de la CPU
                if (CompileStationAndGetErrors(oThisPlc, ref bOneErrorCompilation, ref sErrorText) == false)
                {
                    bRet = false;
                    break;
                }

                break;
            }

            return bRet;
        }


        /// <summary>
        /// Permet de générer le code automate pour un automate passerelle
        /// </summary>
        /// <param name="sStationName"></param>
        /// <returns></returns>
        public bool GenerateTiaCodeForOnePlcGateway(string sStationName, bool bGenerateOnlyInterface, ref string sErrorText)
        {
            bool bRet = true;
            sErrorText = string.Empty;
            PlcSoftware oThisPlc = null;
            PlcBlockUserGroup oThisOPCUAFolder = null;
            Device ostationdevice = null;
            DeviceItem ostationdeviceitem = null;

            while (true)
            {
                // Recherche de la référence de la station passée en paramètre
                oThisPlc = GetThisStationByName(sStationName, ref ostationdevice, ref ostationdeviceitem, ref sErrorText);
                if (oThisPlc == null)
                {
                    bRet = false;
                    break;
                }

                if (bGenerateOnlyInterface == false)
                {

                    // Génération du bloc FC "Test_BLK_JUMP"
                    if (MakeFC_Receive_From_PLC_H(oThisPlc, oThisOPCUAFolder, ref sErrorText) == false)
                    {
                        bRet = false;
                        break;
                    }

                    
                }

                break;
            }

            return bRet;
        }

        /// <summary>
        /// Permet de générer le code automate pour le S7-1500
        /// </summary>
        /// <param name="sStationName"></param>
        /// <returns></returns>
        public bool GenerateTiaCodeForS71500H_R(string sStationName, ref string sErrorText)
        {
            bool bRet = true;
            sErrorText = string.Empty;
            PlcSoftware oThisPlc = null;
            PlcBlockUserGroup oThisOPCUAFolder = null;
            Device ostationdevice = null;
            DeviceItem ostationdeviceitem = null;

            while (true)
            {
                // Recherche de la référence de la station passée en paramètre
                oThisPlc = GetThisStationByName(sStationName, ref ostationdevice, ref ostationdeviceitem, ref sErrorText);
                if (oThisPlc == null)
                {
                    bRet = false;
                    break;
                }
   
                // Test de la présence du folder "BLK_JUMP" et référence sur celle-ci

                oThisOPCUAFolder = GetOrCreateOPCUAFolder(oThisPlc, ref sErrorText);
                if (oThisOPCUAFolder == null)
                {
                    bRet = false;
                    break;
                }

                // Génération du bloc FC "Test_BLK_JUMP"
                if (MakeFC_Receive_From_Gateway(oThisPlc, oThisOPCUAFolder,
                                                sFCName_Receive_From_Gateway_1, sFCTitle_Receive_From_Gateway_1,
                                                sDBName_Global_Mapping_OPC_UA_1, sDBName_Mapping_OPC_UA_From_Gateway_1,
                                                sDBName_Mapping_OPC_UA_From_Gateway_Previous_1,
                                                ref sErrorText) == false)
                {
                    bRet = false;
                    break;
                }

                break;
            }

            return bRet;
        }

        /// <summary>
        /// Permet de récupérer la référence de la station
        /// </summary>
        /// <param name="sStationName"></param>
        /// <returns></returns>
        private PlcSoftware GetThisStationByName(string sStationName, ref Device stationdevice, ref DeviceItem stationdeviceitem, ref string sErrorText)
        {
            PlcSoftware oThisPlc = null;
            List<Device> ListDevice = new List<Device>();
            Device oPLCHStation = null;

            while (true)
            {
                try
                {
                    // Récupération de l'objet station correspondant à l'automate de la station
                    // Lancement de l'énumération des stations dans le projet
                    if (m_oExploreTiaPLCH.GetTiainterface().EnumerationDevice(ref ListDevice, ref sErrorText) == false)
                    {
                        break;
                    }
                    // Recherche de la station H
                    foreach (Device device in ListDevice)
                    {
                        if (device.Name == sStationName)
                        {
                            oPLCHStation = device;
                            stationdevice = device;
                            break;
                        }
                    }
                    // Test si la station a été trouvée ?
                    if (oPLCHStation == null)
                    {
                        sErrorText = string.Format(@"Station '{0}' for Plc Gateway not found !", sStationName);
                        break;
                    }

                    // Récupération du controllertarget
                    DeviceItem deviceItemToGetService = m_oExploreTiaPLCH.TryToFoundDeviceItemInDevice(oPLCHStation);
                    if (deviceItemToGetService == null)
                    {
                        sErrorText = @"Impossible to found PLC device item in device";
                        oThisPlc = null;
                        break;
                    }
                    stationdeviceitem = deviceItemToGetService;
                    SoftwareContainer softwarecontainer = deviceItemToGetService.GetService<SoftwareContainer>() as SoftwareContainer;
                    oThisPlc = softwarecontainer.Software as PlcSoftware;
                    if (oThisPlc == null)
                    {
                        sErrorText = @"controllertarget is null in this device";
                        break;
                    }
                }
                catch (Exception Ex)
                {
                    oPLCHStation = null;
                    sErrorText = string.Format(@"GetThisStationByName() Exception '{0}'", Ex.Message);
                }

                break;
            }

            return oThisPlc;
        }

        /// <summary>
        /// Permet de récupérer la réference sur le folder 
        /// Le répertoire est créer s'il n'existe pas
        /// </summary>
        /// <param name="oThisPlc"></param>
        /// <param name="sErrorText"></param>
        /// <returns></returns>
        PlcBlockUserGroup GetOrCreateOPCUAFolder(PlcSoftware oThisPlc, ref string sErrorText)
        {
            PlcBlockUserGroup oOPCUAGrp = null;
  
            sErrorText = string.Empty;
            bool bRootFound = false;

            while (true)
            {
                try
                {
                    // Recherche du folder "BLK_JUMP" dans la racine du projet (sRootOPCUAFolder)
                    foreach (PlcBlockUserGroup blockUserFolder in oThisPlc.BlockGroup.Groups)
                    {
                        // Test si le folder correspond à celui de BLK_JUMP
                        if (blockUserFolder.Name.ToUpper() == sRootOPCUAFolder.ToUpper())
                        {
                            oOPCUAGrp = blockUserFolder;
                            bRootFound = true;
                            break;
                        }
                    }
                    // Test si la racine BLK_JUMP est trouvée ?

                    if (bRootFound == false)
                    {
                        // Création du folder BLK_JUMP
                        oOPCUAGrp = oThisPlc.BlockGroup.Groups.Create(sRootOPCUAFolder);
                    }
                    
                }
                catch (Exception Ex)
                {
                    oOPCUAGrp = null;
                    sErrorText = string.Format(@"GetOrCreateOPCUAFolder() Exception '{0}'", Ex.Message);
                }

                break;
            }


            return oOPCUAGrp;
        }

        /// <summary>
        /// Permet de creer le bloc FC 
        /// </summary>
        /// <param name="oThisPlc"></param>
        /// <param name="oThisPlcUserFolder"></param>
        /// <param name="sErrorText"></param>
        /// <returns></returns>
        bool MakeFC_Receive_From_PLC_H(PlcSoftware oThisPlc, PlcBlockUserGroup oThisPlcUserFolder, ref string sErrorText)
        {
            bool bRet = true;
            sErrorText = string.Empty;
            List<string> oSourceFC = new List<string>();
            string sFCFileName = string.Empty;
            List<string> oListOfParameters = new List<string>();

            while (true)
            {
                // Création de l'entête du bloc
                oSourceFC.AddRange(MakeFC_Header_For_Receive_From_PLC_H(sFCName_Receive_From_PLC_H, sFCTitle_Receive_From_PLC_H));
                // Ajout du corps code du FC
                oSourceFC.AddRange(CreateFCBodyForReceive_From_PLC_H());
                // Ajout de la fin du bloc
                oSourceFC.AddRange(MakeFC_End());

                // Sauvegarde du FC dans un fichier
                if (CreateFCFile(sFCName_Receive_From_PLC_H, oSourceFC, ref sFCFileName, ref sErrorText) == false)
                {
                    bRet = false;
                    break;
                }
                // Suppression du bloc dans le projet Tia Portal si déja existant dans la répertoire cible
                /*
                if (DeleteBlocFCInTiaPortalProject(oThisPlc, oThisPlcUserFolder, sFCName_Receive_From_PLC_H, ref sErrorText) == false)
                {
                    bRet = false;
                    break;
                }*/

                // Importation de la source du FC dans le projet TIA Portal et génération du bloc dans la bonne arborescence
                if (ImportSourceDBAndGenerateItInTiaPortalProject(oThisPlc, oThisPlcUserFolder, sFCName_Receive_From_PLC_H, sFCFileName, ref sErrorText) == false)
                {
                    bRet = false;
                    break;
                }

                // Suppression du fichier FC
                if (File.Exists(sFCFileName)) File.Delete(sFCFileName);

                break;
            }

            return bRet;
        }

        /// <summary>
        /// Permet de creer l'entête du bloc de type FC Receive_From_PLC_H
        /// </summary>
        /// <param name="sFCName"></param>
        /// <returns></returns>
        List<string> MakeFC_Header_For_Receive_From_PLC_H(string sFCName, string sTitle)
        {
            List<string> oHeader = new List<string>();
            string sLine = string.Empty;


            sLine = string.Format(@"FUNCTION ""{0}"" : Void", sFCName);
            oHeader.Add(sLine);
            sLine = string.Format(@"TITLE = {0}", sTitle);
            oHeader.Add(sLine);
            oHeader.Add(@"{ S7_Optimized_Access := 'TRUE' }");
            oHeader.Add(@"AUTHOR: HMA");
            oHeader.Add(@"FAMILY : Siemens");
            oHeader.Add(@"VERSION : 0.1");
            oHeader.Add(@"BEGIN");

            return oHeader;
        }

        /// <summary>
        /// Permet de creer la fin du bloc FC
        /// </summary>
        /// <returns></returns>
        List<string> MakeFC_End()
        {
            List<string> oEnd = new List<string>();
            string sLine = string.Empty;

            oEnd.Add(@"END_FUNCTION");

            return oEnd;
        }

        /// <summary>
        /// Permet de creer le FC dans le répertoire de l'application
        /// </summary>
        /// <param name="sBlocName"></param>
        /// <returns></returns>
        bool CreateFCFile(string sBlocName, List<string> oFileLines, ref string sFCFileName, ref string sErrorText)
        {
            bool bRet = true;

            sErrorText = string.Empty;

            while (true)
            {
                try
                {
                    // Formatage du nom du fichier FC
                    sFCFileName = string.Format(@"{0}{1}.scl", m_oExploreTiaPLCH.GetTiaProjectDefinitions().sPathApplication, sBlocName);
                    // Effacement du fichier si celui-ci existe
                    if (File.Exists(sFCFileName) == true) File.Delete(sFCFileName);
                    // Sauvegarde du contenu du fichier
                    File.WriteAllLines(sFCFileName, oFileLines.ToArray());

                    break;
                }
                catch (Exception Ex)
                {
                    sErrorText = string.Format(@"Exception in CreateFCFile() : {0}", Ex.Message);
                    bRet = false;
                    break;
                }
            }

            return bRet;
        }

        /// <summary>
        /// Suppression du bloc si présent
        /// </summary>
        /// <param name="sBlocName"></param>
        /// <param name="sErrorText"></param>
        /// <returns></returns>
        bool DeleteBlocInTiaPortalProject(PlcSoftware oThisPlc, PlcBlockUserGroup oThisPlcUserFolder, string sBlocName, ref string sErrorText)
        {
            bool bRet = true;
            List<PlcBlock> oListBlocks = new List<PlcBlock>();

            while (true)
            {
                // Enumération de tous les blocs pour ce répertoire
                if (m_oExploreTiaPLCH.GetTiainterface().EnumerateBlockUserProgramForThisFolder(oThisPlcUserFolder, ref oListBlocks, ref sErrorText) == false)
                {
                    bRet = false;
                    break;
                }
                // Recherche du bloc dans la liste
                foreach (PlcBlock bloc in oListBlocks)
                {
                    // Test si on se toruve sur un bloc DB
                    if (bloc is FC)
                    {
                        // Test si l'on se trouve sur le bon bloc ?
                        if (bloc.Name.ToUpper() == sBlocName.ToUpper())
                        {
                            // On le supprime et on quitte la recherche
                            bloc.Delete();
                            break;
                        }
                    }
                }

                break;
            }

            return bRet;
        }

        /// <summary>
        /// Suppression du bloc de type FC si présent
        /// </summary>
        /// <param name="sBlocName"></param>
        /// <param name="sErrorText"></param>
        /// <returns></returns>
        bool DeleteBlocFCInTiaPortalProject(PlcSoftware oThisPlc, PlcBlockUserGroup oThisPlcUserFolder, string sBlocName, ref string sErrorText)
        {
            bool bRet = true;
            List<PlcBlock> oListBlocks = new List<PlcBlock>();

            while (true)
            {
                // Enumération de tous les blocs pour ce répertoire
                if (m_oExploreTiaPLCH.GetTiainterface().EnumerateBlockUserProgramForThisFolder(oThisPlcUserFolder, ref oListBlocks, ref sErrorText) == false)
                {
                    bRet = false;
                    break;
                }
                // Recherche du bloc dans la liste
                foreach (PlcBlock bloc in oListBlocks)
                {
                    // Test si on se toruve sur un bloc FC
                    if (bloc is FC)
                    {
                        try
                        {
                            // Test si l'on se trouve sur le bon bloc ?
                            if (bloc.Name.ToUpper() == sBlocName.ToUpper())
                            {
                                // On le supprime et on quitte la recherche
                                bloc.Delete();
                                break;
                            }
                        }
                        catch {;}
                    }
                }

                break;
            }

            return bRet;
        }

        /// <summary>
        /// Permet d'importer un bloc source et de le générer dans le bon répertoire cible
        /// </summary>
        /// <param name="oThisPlc"></param>
        /// <param name="oThisBlocFolder"></param>
        /// <param name="sBlocName"></param>
        /// <param name="sDBFileName"></param>
        /// <param name="sErrorText"></param>
        /// <returns></returns>
        bool ImportSourceDBAndGenerateItInTiaPortalProject(PlcSoftware oThisPlc, PlcBlockUserGroup oThisBlocFolder,
                                                           string sBlocName, string sDBFileName, ref string sErrorText)
        {
            bool bRet = true;
            PlcExternalSource source = null;

            while (true)
            {
                try
                {
                    // Incorporation du fichier source dans le projet TIA Portal
                    PlcExternalSourceSystemGroup systemsourcefolder = oThisPlc.ExternalSourceGroup as PlcExternalSourceSystemGroup;
                    // Recherche si la source a intégrer existe afin de la supprimer
                    foreach(PlcExternalSource sourceread in systemsourcefolder.ExternalSources)
                    {
                        if (sourceread.Name.ToUpper() == sBlocName.ToUpper())
                        {
                            // On supprime la source
                            sourceread.Delete();
                            break;
                        }
                    }

                    if (m_oExploreTiaPLCH.GetTiainterface().ImportSourceFileToSourceFolder(systemsourcefolder, sBlocName, sDBFileName, ref source, ref sErrorText) == false)
                    {
                        bRet = false;
                        break;
                    }
                    // Génération du bloc importé
                    if (m_oExploreTiaPLCH.GetTiainterface().GenerateBlocFromSourceFile(source, ref sErrorText) == false)
                    {
                        bRet = false;
                        break;
                    }

                    // Suppression de la source
                    source.Delete();
                    // Déplacement du bloc nouvellement généré dans le répertoire cible
                    if (MoveBlocFromRootFolderToSpecificFolder(oThisPlc, oThisBlocFolder, sBlocName, ref sErrorText) == false)
                    {
                        bRet = false;
                        break;
                    }
                }
                catch (Exception Ex)
                {
                    sErrorText = string.Format(@"Exception in ImportSourceDBAndGenerateItInTiaPortalProject() : {0}", Ex.Message);
                    bRet = false;
                    break;
                }

                break;
            }

            return bRet;
        }


        /// <summary>
        /// Permet de déplacer un bloc du répertoire root vers un répertoire dédié
        /// </summary>
        /// <param name="oThisPlc"></param>
        /// <param name="oThisBlocFolder"></param>
        /// <param name="sBlocName"></param>
        /// <param name="sErrorText"></param>
        /// <returns></returns>
        bool MoveBlocFromRootFolderToSpecificFolder(PlcSoftware oThisPlc, PlcBlockUserGroup oThisBlocFolder, string sBlocName, ref string sErrorText)
        {
            bool bRet = true;
            PlcBlock oBlocToMove = null;
            string sXmlBlocToMove = string.Empty;
            PlcBlock oBlocImport = null;

            while (true)
            {
                try
                {
                    // Recherche du bloc dans la root
                    oBlocToMove = FindAnBlocInSpecificFolder(true, oThisPlc, oThisBlocFolder, sBlocName);
                    if (oBlocToMove == null)
                    {
                        sErrorText = string.Format(@"MoveBlocFromRootFolderToSpecificFolder() Bloc {0} not found", sBlocName);
                        bRet = false;
                        break;
                    }
                    // Avant de lancer l'export du bloc, il faut le compiler
                    if (oBlocToMove.IsConsistent == false)
                    {
                        if (m_oExploreTiaPLCH.GetTiainterface().CompileBloc(oBlocToMove, ref sErrorText) == false)
                        {
                            bRet = false;
                            break;
                        }
                    }

                    // On exporte le bloc en XML pour le reimporter au bon emplacement
                    sXmlBlocToMove = string.Format(@"{0}BlocToMove.XML", m_oExploreTiaPLCH.GetTiaProjectDefinitions().sPathApplication);
                    // Ajout du bloc dans la cible
                    if (m_oExploreTiaPLCH.GetTiainterface().ExportBlocToXml(oBlocToMove, sXmlBlocToMove, ref sErrorText) == false)
                    {
                        bRet = false;
                        break;
                    }
                    // On supprime le bloc de sa source
                    oBlocToMove.Delete();
                    // On réimporte le bloc à son emplacement définitif
                    if (m_oExploreTiaPLCH.GetTiainterface().ImportBlocFromXml(oThisBlocFolder.Blocks, sXmlBlocToMove, ref sErrorText) == false)
                    {
                        bRet = false;
                        break;
                    }
                    // Suppression du fichier XML source
                    if (File.Exists(sXmlBlocToMove) == true) File.Delete(sXmlBlocToMove);

                    // Après importation on recompile le bloc ainsi importé
                    // Recherche du bloc importé dans le répertoire cible 
                    oBlocImport = FindAnBlocInSpecificFolder(false, oThisPlc, oThisBlocFolder, sBlocName);
                    if (oBlocImport == null)
                    {
                        sErrorText = string.Format(@"MoveBlocFromRootFolderToSpecificFolder() Bloc {0} not found", sBlocName);
                        bRet = false;
                        break;
                    }
                    // On lance la compilation du du bloc après importation
                    if (oBlocImport.IsConsistent == false)
                    {
                        if (m_oExploreTiaPLCH.GetTiainterface().CompileBloc(oBlocImport, ref sErrorText) == false)
                        {
                            bRet = false;
                            break;
                        }
                    }


                }
                catch (Exception Ex)
                {
                    sErrorText = string.Format(@"Exception in MoveBlocFromRootFolderToSpecificFolder() : {0}", Ex.Message);
                    bRet = false;
                    break;
                }
                break;
            }

            return bRet;
        }

        /// <summary>
        /// Permet de rechercher un bloc spécifique dans un répertoire spécifique
        /// </summary>
        /// <param name="bInRoot"> True si recherche dans la root </param>
        /// <param name="oThisPlc"></param>
        /// <param name="oThisBlocFolder"></param>
        /// <param name="sBlocName"></param>
        /// <returns></returns>
        PlcBlock FindAnBlocInSpecificFolder(bool bInRoot, PlcSoftware oThisPlc, PlcBlockUserGroup oThisBlocFolder, string sBlocName)
        {
            PlcBlock oBlocToFound = null;

            while(true)
            {
                switch(bInRoot)
                {
                    case false:
                        foreach (PlcBlock bloc in oThisBlocFolder.Blocks)
                        {
                            if (bloc.Name.ToUpper() == sBlocName.ToUpper())
                            {
                                // On se trouve sur le bon bloc
                                oBlocToFound = bloc;
                                break;
                            }
                        }
                        break;
                    case true:
                        foreach (PlcBlock bloc in oThisPlc.BlockGroup.Blocks)
                        {
                            if (bloc.Name.ToUpper() == sBlocName.ToUpper())
                            {
                                // On se trouve sur le bon bloc
                                oBlocToFound = bloc;
                                break;
                            }
                        }
                        break;
                }

                break;
            }

            return oBlocToFound;
        }

        /// <summary>
        /// Permet de créer le corps du FC au niveau instruction pour Receive_From_PLC_H
        /// </summary>
        /// <returns></returns>
        List<string> CreateFCBodyForReceive_From_PLC_H()
        {
            List<string> olist = new List<string>();
            string sLineParameter = string.Empty;
            string sCurrentVariablePathWithoutDoubleCote = string.Empty;

            sLineParameter = @"// Test si une nouvelle trame est arrivée ?";
            olist.Add(sLineParameter);
            sLineParameter = string.Format(@"IF (""{0}"".Previous_KeyExchange_From_PLC_H <> ""{1}"".Frame.KeyExchange) THEN",
                                           sDBName_Global_Mapping_OPC_UA, sDBName_Mapping_OPC_UA_From_PLC_H);
            olist.Add(sLineParameter);

            sLineParameter = AddSpaceBeginStr(@"// Mémorisation de la clé", 4);
            olist.Add(sLineParameter);
            sLineParameter = AddSpaceBeginStr(string.Format(@"""{0}"".Previous_KeyExchange_From_PLC_H := ""{1}"".Frame.KeyExchange;",
                                                            sDBName_Global_Mapping_OPC_UA, sDBName_Mapping_OPC_UA_From_PLC_H), 4);
            olist.Add(sLineParameter);

            sLineParameter = AddSpaceBeginStr(@"// Mise à jour de la zone mémoire interne de la CPU", 4);
            olist.Add(sLineParameter);

            // Boucle de traitement de tous les blocs concernés
            foreach (TiaPortalBloc tiabloc in m_oTiaProjectForCPUH.oListblocs)
            {
                foreach (TiaPortalVariable tiavariable in tiabloc.VariablesList)
                {
                    // Test si un chemin est indiqué pour cette variable
                    if (tiavariable.CurrentPathVariable.Length == 0)
                    {
                        #region Modifs_17/02/2021  
                        /* Suite modifications du 17/12/2021
                                              sLineParameter = AddSpaceBeginStr(string.Format(@"IF (""{0}"".Frame.""{1}.{2}"" <> ""{3}"".Frame.""{4}.{5}"") THEN",
                                                                                              sDBName_Mapping_OPC_UA_From_PLC_H,
                                                                                              tiabloc.Name, tiavariable.Name,
                                                                                              sDBName_Mapping_OPC_UA_From_PLC_H_Previous,
                                                                                              tiabloc.Name, tiavariable.Name), 4);
                                              olist.Add(sLineParameter);
                                              sLineParameter = AddSpaceBeginStr(string.Format(@"""{0}"".Frame.""{1}.{2}"" := ""{3}"".Frame.""{4}.{5}"";",
                                                                                              sDBName_Mapping_OPC_UA_From_PLC_H_Previous,
                                                                                              tiabloc.Name, tiavariable.Name,
                                                                                              sDBName_Mapping_OPC_UA_From_PLC_H,
                                                                                              tiabloc.Name, tiavariable.Name), 8);
                                              olist.Add(sLineParameter);
                                              sLineParameter = AddSpaceBeginStr(string.Format(@"""{0}"".""{1}.{2}"" := ""{3}"".Frame.""{4}.{5}"";",
                                                                                              sDBName_Mapping_OPC_Server,
                                                                                              tiabloc.Name, tiavariable.Name,
                                                                                              sDBName_Mapping_OPC_UA_From_PLC_H,
                                                                                              tiabloc.Name, tiavariable.Name), 8);
                                              olist.Add(sLineParameter);
                                              sLineParameter = AddSpaceBeginStr(@"END_IF;", 4);
                                              olist.Add(sLineParameter);
                      */
                        #endregion

                        sLineParameter = AddSpaceBeginStr(string.Format(@"""{0}"".""{1}.{2}"" := ""{3}"".Frame.""{4}.{5}"";",
                                                                        sDBName_Mapping_OPC_Server,
                                                                        tiabloc.Name, tiavariable.Name,
                                                                        sDBName_Mapping_OPC_UA_From_PLC_H,
                                                                        tiabloc.Name, tiavariable.Name), 8);
                        olist.Add(sLineParameter);



                    }
                    else
                    {
                        sCurrentVariablePathWithoutDoubleCote = tiavariable.CurrentPathVariable.Replace(@"""", string.Empty) + "." + tiavariable.Name;

                        #region Modifs_17/02/2021                        
                        /* Suite modifications du 17/12/2021
                                                sLineParameter = AddSpaceBeginStr(string.Format(@"IF (""{0}"".Frame.""{1}.{2}"" <> ""{3}"".Frame.""{4}.{5}"") THEN",
                                                                                                sDBName_Mapping_OPC_UA_From_PLC_H,
                                                                                                tiabloc.Name, sCurrentVariablePathWithoutDoubleCote,
                                                                                                sDBName_Mapping_OPC_UA_From_PLC_H_Previous,
                                                                                                tiabloc.Name, sCurrentVariablePathWithoutDoubleCote), 4);
                                                olist.Add(sLineParameter);
                                                sLineParameter = AddSpaceBeginStr(string.Format(@"""{0}"".Frame.""{1}.{2}"" := ""{3}"".Frame.""{4}.{5}"";",
                                                                                                sDBName_Mapping_OPC_UA_From_PLC_H_Previous,
                                                                                                tiabloc.Name, sCurrentVariablePathWithoutDoubleCote,
                                                                                                sDBName_Mapping_OPC_UA_From_PLC_H,
                                                                                                tiabloc.Name, sCurrentVariablePathWithoutDoubleCote), 8);
                                                olist.Add(sLineParameter);
                                                sLineParameter = AddSpaceBeginStr(string.Format(@"""{0}"".""{1}.{2}"" := ""{3}"".Frame.""{4}.{5}"";",
                                                                                                sDBName_Mapping_OPC_Server,
                                                                                                tiabloc.Name, sCurrentVariablePathWithoutDoubleCote,
                                                                                                sDBName_Mapping_OPC_UA_From_PLC_H,
                                                                                                tiabloc.Name, sCurrentVariablePathWithoutDoubleCote), 8);
                                                olist.Add(sLineParameter);
                                                sLineParameter = AddSpaceBeginStr(@"END_IF;", 4);
                                                olist.Add(sLineParameter);
                        */
                        #endregion

                        sLineParameter = AddSpaceBeginStr(string.Format(@"""{0}"".""{1}.{2}"" := ""{3}"".Frame.""{4}.{5}"";",
                                                                        sDBName_Mapping_OPC_Server,
                                                                        tiabloc.Name, sCurrentVariablePathWithoutDoubleCote,
                                                                        sDBName_Mapping_OPC_UA_From_PLC_H,
                                                                        tiabloc.Name, sCurrentVariablePathWithoutDoubleCote), 8);
                        olist.Add(sLineParameter);

                    }

                    #region Modifs
                    /*
                                        sLineParameter = AddSpaceBeginStr(string.Format(@"IF (""{0}"".Frame.""{1}.{2}"" <> ""{3}"".Frame.""{4}.{5}"") THEN",
                                                                                        sDBName_Mapping_OPC_UA_From_PLC_H,
                                                                                        tiabloc.Name, tiavariable.Name,
                                                                                        sDBName_Mapping_OPC_UA_From_PLC_H_Previous,
                                                                                        tiabloc.Name, tiavariable.Name), 4);
                                        olist.Add(sLineParameter);
                                        sLineParameter = AddSpaceBeginStr(string.Format(@"""{0}"".Frame.""{1}.{2}"" := ""{3}"".Frame.""{4}.{5}"";",
                                                                                        sDBName_Mapping_OPC_UA_From_PLC_H_Previous,
                                                                                        tiabloc.Name, tiavariable.Name,
                                                                                        sDBName_Mapping_OPC_UA_From_PLC_H,
                                                                                        tiabloc.Name, tiavariable.Name), 8);
                                        olist.Add(sLineParameter);
                                        sLineParameter = AddSpaceBeginStr(string.Format(@"""{0}"".""{1}.{2}"" := ""{3}"".Frame.""{4}.{5}"";",
                                                                                        sDBName_Mapping_OPC_Server,
                                                                                        tiabloc.Name, tiavariable.Name,
                                                                                        sDBName_Mapping_OPC_UA_From_PLC_H,
                                                                                        tiabloc.Name, tiavariable.Name), 8);
                                        olist.Add(sLineParameter);
                                        sLineParameter = AddSpaceBeginStr(@"END_IF;", 4);
                                        olist.Add(sLineParameter);
                    */
                    #endregion
                }
                }

            // Boucle de traitement de tous les tags concernés
            foreach (TiaPortalVariable tiavariable in m_oTiaProjectForCPUH.oListTagsVariables)
            {
                #region Modifs_17/02/2021                        

                /* Suite modifications du 17/12/2021
                              sLineParameter = AddSpaceBeginStr(string.Format(@"IF (""{0}"".Frame.""{1}"" <> ""{2}"".Frame.""{3}"") THEN",
                                                                              sDBName_Mapping_OPC_UA_From_PLC_H,
                                                                              tiavariable.Name,
                                                                              sDBName_Mapping_OPC_UA_From_PLC_H_Previous,
                                                                              tiavariable.Name), 4);
                              olist.Add(sLineParameter);
                              sLineParameter = AddSpaceBeginStr(string.Format(@"""{0}"".Frame.""{1}"" := ""{2}"".Frame.""{3}"";",
                                                                              sDBName_Mapping_OPC_UA_From_PLC_H_Previous,
                                                                              tiavariable.Name,
                                                                              sDBName_Mapping_OPC_UA_From_PLC_H,
                                                                              tiavariable.Name), 8);
                              olist.Add(sLineParameter);
                              sLineParameter = AddSpaceBeginStr(string.Format(@"""{0}"".""{1}"" := ""{2}"".Frame.""{3}"";",
                                                                              sDBName_Mapping_OPC_Server,
                                                                              tiavariable.Name,
                                                                              sDBName_Mapping_OPC_UA_From_PLC_H,
                                                                              tiavariable.Name), 8);
                              olist.Add(sLineParameter);
                              sLineParameter = AddSpaceBeginStr(@"END_IF;", 4);
                              olist.Add(sLineParameter);
              */
                #endregion

                sLineParameter = AddSpaceBeginStr(string.Format(@"""{0}"".""{1}"" := ""{2}"".Frame.""{3}"";",
                                                                sDBName_Mapping_OPC_Server,
                                                                tiavariable.Name,
                                                                sDBName_Mapping_OPC_UA_From_PLC_H,
                                                                tiavariable.Name), 8);
                olist.Add(sLineParameter);

            }
            
            sLineParameter = @"END_IF;";
            olist.Add(sLineParameter);

            return olist;
        }

        /// <summary>
        /// Permet de compiler la station complète et de vérifier si pas d'erreur lors de la compilation
        /// </summary>
        /// <param name="oThisPlc"></param>
        /// <param name="sErrorText"></param>
        /// <returns></returns>
        bool CompileStationAndGetErrors(PlcSoftware oThisPlc, ref bool bAtLeastOneError, ref string sErrorText)
        {
            bool bRet = true;
            bAtLeastOneError = false;

            while (true)
            {
                try
                {
                    ICompilable compileService = oThisPlc.GetService<ICompilable>();
                    CompilerResult result = compileService.Compile();
                    if (result.ErrorCount != 0) bAtLeastOneError = true;
                }
                catch (Exception Ex)
                {
                    bRet = false;
                    bAtLeastOneError = true;
                    sErrorText = string.Format(@"CompileStation Exception '{0}'", Ex.Message);
                    break;
                }
                // Test si des erreurs de compilation
                if (bAtLeastOneError == true)
                {
                    sErrorText = @"Compilation errors found in station";
                    bRet = false;
                }

                break;
            }

            return bRet;

        }
 
        /// <summary>
        /// Permet de creer le bloc FC 
        /// </summary>
        /// <param name="oThisPlc"></param>
        /// <param name="oThisPlcUserFolder"></param>
        /// <param name="s_DBName_Global_Mapping_OPC_UA"></param>
        /// <param name="s_DBName_Mapping_OPC_UA_From_Gateway"></param>
        /// <param name="s_DBName_Mapping_OPC_UA_From_Gateway_Previous"></param>
        /// <param name="sErrorText"></param>
        /// <returns></returns>
        bool MakeFC_Receive_From_Gateway(PlcSoftware oThisPlc, PlcBlockUserGroup oThisPlcUserFolder,
                                         string s_FCName_Receive_From_Gateway, string s_sFCTitle_Receive_From_Gateway,
                                         string s_DBName_Global_Mapping_OPC_UA, string s_DBName_Mapping_OPC_UA_From_Gateway,
                                         string s_DBName_Mapping_OPC_UA_From_Gateway_Previous,
                                         ref string sErrorText)
        {
            bool bRet = true;
            sErrorText = string.Empty;
            List<string> oSourceFC = new List<string>();
            string sFCFileName = string.Empty;
            List<string> oListOfParameters = new List<string>();

            DateTime localDate = DateTime.Now;

            while (true)
            {
 
                // Création de l'entête du bloc
                oSourceFC.AddRange(MakeFC_Header_For_Receive_From_PLC_H(s_FCName_Receive_From_Gateway, s_sFCTitle_Receive_From_Gateway));

                //Ajout dans le corps de la fonction
                oSourceFC.Add(@"// BLOC FONCTION D'AIDE AU DIAGNOSTIC");
                oSourceFC.Add(@"//");
                oSourceFC.Add(@"// Ce bloc permet de recepurer toutes les pattes d'entree se trouvant");
                oSourceFC.Add(@"// dans le fichier excel : Liste_LTU_BackJump.xlsx");
                oSourceFC.Add(@"//");
                oSourceFC.Add(@"// DATE ET HEURE DE LA DERNIERE EXECUTION");
                oSourceFC.Add(@"// DE LA FONCTION :" + localDate);
                oSourceFC.Add(@""); 
                oSourceFC.Add(@"");



                bool test = false;
                
                foreach (string element in m_odataCollection)
                {
                    if (element.Substring(0, 1) != "/")
                    {
                        test = true;
                        break;
                    }
                }
               

                if (m_odataCollection.Count == 0 || test == false)
                {
                    oSourceFC.Add(@"//");
                    oSourceFC.Add(@"// AUCUNE SOURCE LTU DETECTEE");
                    oSourceFC.Add(@"//");
                }
                else
                {

                    oSourceFC.Add(@"IF (""FirstScan"" OR ""EnableBackJump"") THEN");

                    //Ecriture du resultat de recherche
                    foreach (string element in m_odataCollection)
                    {
                        oSourceFC.Add(@element);
                    }

                    oSourceFC.Add(@"END_IF;");

                }

                // Ajout de la fin du bloc
                oSourceFC.AddRange(MakeFC_End());
                
                // Sauvegarde du FC dans un fichier
                if (CreateFCFile(s_FCName_Receive_From_Gateway, oSourceFC, ref sFCFileName, ref sErrorText) == false)
                {
                    bRet = false;
                    break;
                }
                // Suppression du bloc dans le projet Tia Portal si déja existant dans la répertoire cible
                if (DeleteBlocFCInTiaPortalProject(oThisPlc, oThisPlcUserFolder, s_FCName_Receive_From_Gateway, ref sErrorText) == false)
                {
                    bRet = false;
                    break;
                }

                // Importation de la source du FC dans le projet TIA Portal et génération du bloc dans la bonne arborescence
                if (ImportSourceDBAndGenerateItInTiaPortalProject(oThisPlc, oThisPlcUserFolder, s_FCName_Receive_From_Gateway, sFCFileName, ref sErrorText) == false)
                {
                    bRet = false;
                    break;
                }

                // Suppression du fichier FC
                if (File.Exists(sFCFileName)) File.Delete(sFCFileName);

                break;
            }

            return bRet;
        }

    }

}
