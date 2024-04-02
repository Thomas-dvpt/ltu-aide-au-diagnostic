using GlobalsOPCUAH;
using Siemens.Engineering.HW.Features;
using Siemens.Engineering.HW;
using Siemens.Engineering.SW;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using TiaExplorePLCH;
using ClosedXML;
using ClosedXML.Excel;




/// <summary>
/// 
/// Createur : Youssef EL BAZI Siemens DI CS France
/// Date : 10/02/2023
/// Description : Remplacement de la fonction GetSymbolPath 
///               dans TIA portal
///
/// 
/// </summary>

namespace GenerateBLK
{
    public partial class GenerateBLKForm : Form
    {
        #region Variables

        private bool m_bFirstCall = false;

        // Debugview activation trace
        private bool _bWithDebugView = false;
        public bool bWithDebugView { get { return _bWithDebugView; } }

        // objet trace pour l'application principale
        private CsHMATrace m_oTrace = new CsHMATrace("GenerateBLK", true);

        // Indication démarrage de l'application
        private bool m_bApplicationIsStarted = false;

        // Indication erreur lors du démarrage
        private bool m_bErrorWhenStarted = false;

        // Partie raffraichissement information
        private bool m_bThreadRefreshFinish = false;

        // Objet thread de démarrage
        private Thread m_oThreadStartApplication;

        // Objet evenement de notification
        public event EventHandler TaskEventEndThreadStartApplication;

        // Paramètres généraux pour l'application
        public static PLC_H_ProjectDefinitions m_oPLC_H_ProjectDefinitions = new PLC_H_ProjectDefinitions();

        // Objet interface Tia Portal
        private ExploreTiaPLCH m_oExploreTiaPLCH;

        // Objet de définition de la CPU H
        private TiaProjectForCPUH m_oTiaProjectForCPUH;

        // Objet de génération du code pour les automates gateway et H
        private PlcGenerateTiaCode m_oPlcGenerateTiaCode;

        // Objet evenement de notification de fin de génération
        public event EventHandler TaskEventEndThreadGenerationCode;

        // Indication démarrage generation de code
        private bool m_bCodeGenerationIsStarted = false;

        // Indication erreur lors de la génération
        private bool m_bErrorWhenGeneratedCode = false;

        // Objet thread de génération
        private Thread m_oThreadStartGeneratePLC;

        private string m_sOldTraceInformation = string.Empty;

        //        public static CsThreadProcessCode m_oCsThreadProcessCode;

        private Dictionary<Tuple<int, int>, object> data = new Dictionary<Tuple<int, int>, object>();

        //Liste des instructions pour le FC
        private List<string> dataCollection = new List<string>();

        #endregion


        /// <summary>
        /// Méthode principale de lancement de l'application
        /// </summary>
        public GenerateBLKForm()
        {
            // Chargement du splasscreen
            Thread t = new Thread(new ThreadStart(SplashScreenStart));
            t.Start();
            Thread.Sleep(5000);
            t.Abort();
            InitializeComponent();
            // Resolution des assemblages pour obtenir ceux d'Openness
            AppDomain.CurrentDomain.AssemblyResolve += MyResolver;
            BackgroundWorker.WorkerReportsProgress = true;
            BackgroundWorker.DoWork += new DoWorkEventHandler(Backgroundworker_DoWork);
            BackgroundWorker.ProgressChanged += new ProgressChangedEventHandler(BackgroundWorker_ProgressChanged);
        }

        /// <summary>
        /// Permet de résoudre le chemin pour l'assemblage de développement TIA Portal
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        private static Assembly MyResolver(object sender, ResolveEventArgs args)
        {
            int index = args.Name.IndexOf(',');
            if (index == -1)
            {
                return null;
            }
            string name = args.Name.Substring(0, index) + ".dll";
            string path = Path.Combine(m_oPLC_H_ProjectDefinitions.GetOpennessLibraryPath(), name);
            // User must provide the correct path
            string fullPath = Path.GetFullPath(path);
            if (File.Exists(fullPath))
            {
                return Assembly.LoadFrom(fullPath);
            }
            return null;
        }


        /// <summary>
        /// Chargement du splashscreen
        /// </summary>
        public void SplashScreenStart()
        {
            Application.Run(new HMASplashScreen());
        }

        /// <summary>
        /// Permet de lancer le timer de raffraichissement de la progressbar
        /// </summary>
        /// <param name="bStart"></param>
        private void StartStopProgressInformation(bool bStart)
        {
            if (bStart == true)
            {
                m_bThreadRefreshFinish = false;
                BackgroundWorker.RunWorkerAsync();
                Gif_Wait.Visible = true;
            }
            else
            {
                m_bThreadRefreshFinish = true;
                BackgroundWorker.CancelAsync();
                StatusProgressbar.Value = 0;
                Gif_Wait.Visible = false;
                Application.DoEvents();
            }
        }

        /// <summary>
        /// Methode de traitement du thread de tache de fond pour le raffraichissement
        /// de la barre de progression
        /// </summary>
        /// <param name="Sender"></param>
        /// <param name="e"></param>
        void Backgroundworker_DoWork(object Sender, DoWorkEventArgs e)
        {
            int i = 0;
            while (m_bThreadRefreshFinish == false)
            {
                BackgroundWorker.ReportProgress(i);
                Thread.Sleep(500);
                i = i + 10;
                if (i > 100) i = 0;
            }
        }

        /// <summary>
        /// Méthode de mise à jour de la barre de progression
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void BackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            StatusProgressbar.Value = e.ProgressPercentage;
        }

        /// <summary>
        /// Evenement d'affichage de l'application
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GenerateBLKForm_Shown(object sender, EventArgs e)
        {
            // Test si premier appel de l'application
            if (m_bFirstCall == false)
            {
                m_bErrorWhenStarted = false;
                LogInformation(@"Démarrage de l'application...");
                StatusLabel.Text = "Application is starting, please wait for loading configuration...";
                // Chargement de l'application
                // Lancement du processus de l'application
                TaskEventEndThreadStartApplication += ThreadConnectionIsFinishedTask;
                m_oThreadStartApplication = new Thread(() => StartApplication());
                try
                {
                    m_bApplicationIsStarted = false;
                    StartStopProgressInformation(true);

                    // Lancement du thread
                    m_oThreadStartApplication.Start();

                    while (m_bApplicationIsStarted == false)
                    {
                        Application.DoEvents();

                    }
                }
                finally
                {
                    StartStopProgressInformation(false);
                    // Test si une erreur est apparue pendant le démarrage
                    if (m_bErrorWhenStarted == true)
                    {
                        // On quitte l'application
                        Environment.Exit(0);
                    }
                    else
                    {
                        StatusLabel.Text = "Application is loaded correctly";
                        this.Activate();
                    }
                }
                m_bFirstCall = true;
            }
        }

        /// <summary>
        /// Permet de tracer dans l'application depuis le thread auxilliaire
        /// </summary>
        /// <param name="sTrace"></param>
        public void TraceThreadUIPrincipal(string sTrace)
        {
            if (Status.InvokeRequired)
            {
                Status.BeginInvoke(new MethodInvoker(() => LogInformation(sTrace)));
            }
            else
            {
                LogInformation(sTrace);
            }
        }

        /// <summary>
        /// Envoi des informations dans la trace
        /// </summary>
        /// <param name="sTrace"></param>
        private void LogInformation(string sTrace)
        {
            m_sOldTraceInformation = sTrace;
            if (InfoTrace.Lines.Count() > 500) InfoTrace.Clear();
            if (InfoTrace.Lines.Count() == 0) InfoTrace.AppendText(sTrace);
            else InfoTrace.AppendText(Environment.NewLine + sTrace);

            InfoTrace.Update();
            InfoTrace.Refresh();
            Application.DoEvents();

            // Test si debugview est actif ?
            if (bWithDebugView == true)
            {
                m_oTrace.HMATrace(sTrace, CsHMATrace.leveltype.level1);
            }
        }

        /// <summary>
        /// Permet de tracer dans l'application depuis le thread auxilliaire
        /// </summary>
        /// <param name="sTrace"></param>
        public void LogInformation_ThreadUIPrincipal(string sTrace)
        {
            if (Status.InvokeRequired)
            {
                Status.BeginInvoke(new MethodInvoker(() => LogInformation(sTrace)));
            }
            else
            {
                LogInformation(sTrace);
            }
        }


        /// <summary>
        /// Evenement de fin de démarrage de l'application
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="arg"></param>
        private void ThreadConnectionIsFinishedTask(object sender, EventArgs arg)
        {
            m_bApplicationIsStarted = true;
        }

        /// <summary>
        /// Permet d'envoyer des informations dans la barre de status
        /// </summary>
        /// <param name="sTrace"> Texte envoyé </param>
        private void Trace(string sTrace)
        {
            Status.Text = sTrace;
        }

        /// <summary>
        /// Permet de lancer l'application
        /// </summary>
        private void StartApplication()
        {
            string sError = string.Empty;

            m_oTrace.HMATrace("Application is starting...", CsHMATrace.leveltype.level1);

            // Lecture du fichier ini de configuration
            TraceThreadUIPrincipal(@"Lecture du fichier de configuration...");
            ReadIniFile();

            //LECTURE DU FICHIER EXCEL A L'INIT DE L'APP
            ReadExcel();
            TraceThreadUIPrincipal(@"Lecture du fichier excel : OK");

            // Initialisation de tous les objets internes
            m_oExploreTiaPLCH = new ExploreTiaPLCH(m_oPLC_H_ProjectDefinitions, data , dataCollection);
            m_oPlcGenerateTiaCode = new PlcGenerateTiaCode(m_oExploreTiaPLCH, dataCollection);

            // Indication de fin du thread
            this.BeginInvoke(TaskEventEndThreadStartApplication, EventArgs.Empty);
        }


        /// <summary>
        /// Permet de lire le fichier de configuration de l'application
        /// </summary>
        private void ReadIniFile()
        {
            string sTemp = "";

            IniFile oIniFile = new IniFile(AppDomain.CurrentDomain.BaseDirectory + @"\Using_Files\HMAOPCUAH.ini");

            // Lecture du chemin d'installation de l'application
            m_oPLC_H_ProjectDefinitions.sPathApplication = AppDomain.CurrentDomain.BaseDirectory;

            // Lecture si debugview est utilisé pour les traces
            sTemp = oIniFile.Read("WithDebugView", "GlobalParameters");
            try
            {
                if (sTemp.Length == 0) _bWithDebugView = false;
                else
                {
                    int iValid = int.Parse(sTemp);
                    if (iValid > 0) _bWithDebugView = true;
                }
            }
            catch
            {
                _bWithDebugView = false;
            }

            //*****************************************
            // Read Tia portal settings
            //*****************************************

            // Read if tia Portal interface is visible
            sTemp = oIniFile.Read("WithUITiaPortal", "TIAPORTAL");
            try
            {
                if (sTemp.Length == 0) m_oPLC_H_ProjectDefinitions.bWithUITiaPortal = false;
                else
                {
                    int iValid = int.Parse(sTemp);
                    if (iValid > 0) m_oPLC_H_ProjectDefinitions.bWithUITiaPortal = true;
                }
            }
            catch
            {
                m_oPLC_H_ProjectDefinitions.bWithUITiaPortal = false;
            }
            // Read TIA Poral openness assemblies path
            m_oPLC_H_ProjectDefinitions.SetOpennessLibraryPath(oIniFile.Read("FolderOpennessAssemblies", "TIAPORTAL"));
            // Read Extension project name Tia Portal
            m_oPLC_H_ProjectDefinitions.sExtensionProjectName = oIniFile.Read("ExtensionProjectName", "TIAPORTAL");

            //*****************************************
            // Read Project definitions
            //*****************************************
            // Read Mark OPC UA search string for Family Bloc
            m_oPLC_H_ProjectDefinitions.sFamilyBlocMarck = oIniFile.Read("FamilyBlocMarckDef", "Project_Definitions");
            // Read String parameter define new bloc name
            m_oPLC_H_ProjectDefinitions.sFamillyStrResearchNewBlocName = oIniFile.Read("FamillyStrResearchNewBlocNameDef", "Project_Definitions");
            // Read Mark OPC UA search string for Commentar Bloc Parameter
            m_oPLC_H_ProjectDefinitions.sCommentarBlocParameterMarck = oIniFile.Read("CommentarBlocParameterMarckDef", "Project_Definitions");
            // Read Mark OPC UA search string for Commentar Tag variable
            m_oPLC_H_ProjectDefinitions.sCommentarTagVariableMarck = oIniFile.Read("CommentarTagVariableMarckDef", "Project_Definitions");

            //*****************************************
            // Read OPCUA_Server Specification
            //*****************************************
            // Read Root Folder for Blocks in OPC UA server
            m_oPLC_H_ProjectDefinitions.sRootFolderBlocOPCUAServer = oIniFile.Read("RootFolderBlocOPCUAServer", "OPCUA_Server_Specification");
            // Read Root Folder for Tags in OPC UA server
            m_oPLC_H_ProjectDefinitions.sRootFolderTagsOPCUAServer = oIniFile.Read("RootFolderTagsOPCUAServer", "OPCUA_Server_Specification");

            //*****************************************
            // Read DB OPC UA mapping
            //*****************************************
            // Read DB OPC UA mapping name
            m_oPLC_H_ProjectDefinitions.sDBNameMappingOPCUA = oIniFile.Read("DBNameMappingOPCU", "OPCUA_Server_Specification");

            //*****************************************
            // Read Namespace for OPC UA Server
            //*****************************************
            // Read namespace OPC UA server
            m_oPLC_H_ProjectDefinitions.sOPCUAServerNamespace = oIniFile.Read("OPCUAServerNamespace", "OPCUA_Server_Specification");

            //*************************************************************
            // Read flag to validate new name familly only for node id tag
            //*************************************************************
            // Read flag to validate new name familly only for node id tag
            sTemp = oIniFile.Read("NewNameBlockOnlyForTagNodeId", "OPCUA_Server_Specification");
            try
            {
                if (sTemp.Length == 0) m_oPLC_H_ProjectDefinitions.bNewNameBlockOnlyForTagNodeId = false;
                else
                {
                    int iValid = int.Parse(sTemp);
                    if (iValid > 0) m_oPLC_H_ProjectDefinitions.bNewNameBlockOnlyForTagNodeId = true;
                }
            }
            catch
            {
                m_oPLC_H_ProjectDefinitions.bNewNameBlockOnlyForTagNodeId = false;
            }


            //****************************************************
            // Read Credentials informations for Tia Portal access
            //****************************************************
            // Read Username to access Tia Portal project
            m_oPLC_H_ProjectDefinitions.SetUserName(oIniFile.Read("UserName", "Credentials"));
            // Read Crypt password for user to access Tia portal project
            m_oPLC_H_ProjectDefinitions.SetUncryptPasswordUser(oIniFile.Read("Password", "Credentials"));


        }

        //************************    LECTURE FICHIER EXCEL    *****************************\\
        private void ReadExcel()
        {

            //Chemin dans l'application en cours d'exe
            string filePath = AppDomain.CurrentDomain.BaseDirectory + @"Liste_LTU_BackJump.xlsx";

            //CheminFixe
            //string filePath = @"C:\BLK_JUMP\Excel\Liste_LTU_BackJump.xlsx";

            //Dictionary<Tuple<int, int>, object> data = new Dictionary<Tuple<int, int>, object>();

            using (XLWorkbook wb = new XLWorkbook(filePath))
            {
                var ws = wb.Worksheets.First();
                var range = ws.RangeUsed();

                for (int i = 1; i < range.RowCount() + 1; i++)
                {
                    for (int j = 1; j < range.ColumnCount() + 1; j++)
                    {

                        if (j == 1)
                        {
                            if (ws.Cell(i, 2).IsEmpty())
                            {
                                break;
                            }
                        }

                        data.Add(new Tuple<int, int>(i, j), ws.Cell(i, j).Value);

                    }

                }
            }

        }

        private void ToolStripMenuClearTrace_Click(object sender, EventArgs e)
        {
            InfoTrace.Clear();
        }

        private void BpSelectS71500H_Click(object sender, EventArgs e)
        {
            string sStation = string.Empty;

            if (GetS71500H_Station(ref sStation) == true)
            {
                LogInformation(@"La station cible est bien affectée");
                m_oPLC_H_ProjectDefinitions.sPLCS71500HTargetStationName = sStation;
                PLC_H_Station_Target_Name.Text = sStation;
                
            }
            else
            {
                LogInformation(@"La station cible n'est pas affectée");
                m_oPLC_H_ProjectDefinitions.sPLCS71500HTargetStationName = string.Empty;
                PLC_H_Station_Target_Name.Text = string.Empty;
            }
        }

        private void BpSelectS71500_Click(object sender, EventArgs e)
        {
            string sStation = string.Empty;

            if (GetS71500_Gateway_Station(ref sStation) == true)
            {
                LogInformation(@"PLC S7-1500 Gateway (1) station affected");
                m_oPLC_H_ProjectDefinitions.sPLCS71500GatewayStationName_01 = sStation;
                //PLC_Station_Gateway_Name_01.Text = sStation;
            }
            else
            {
                LogInformation(@"PLC S7-1500 Gateway (2) station not affected");
                m_oPLC_H_ProjectDefinitions.sPLCS71500GatewayStationName_01 = string.Empty;
                //PLC_Station_Gateway_Name_01.Text = string.Empty;
            }
        }

        /// <summary>
        /// Permet de sélectionner la station S7-1500
        /// </summary>
        /// <returns></returns>
        private bool GetS71500H_Station(ref string sStationName)
        {
            bool bRet = true;
            string sError = string.Empty;

            sStationName = string.Empty;

            while (true)
            {
                // Test si le projet Tia Portal est déja sélectionné ?
                if (m_oExploreTiaPLCH.GetTiaPortalProjectIsSelected() == false)
                {
                    // Sélection du projet Tia Portal
                    if (m_oExploreTiaPLCH.ChooseTiaProject(ref sError) == false)
                    {
                        bRet = false;
                        break;
                    }
                    // On ouvre le projet Tia Portal associé
                    
                }
                // Récupération de la station S7-1500 H dans le projet sélectionné
                if (m_oExploreTiaPLCH.GetTiaStationFromProjectList(ref sStationName, ref sError) == false)
                {
                    bRet = false;
                }

                break;
            }

            // Affichage du nom du projet selectionné
            string projet = m_oExploreTiaPLCH.m_oTiainterface.m_oTiaProject.Name;
            textBox1.Text = projet;

            return bRet;
        }

        /// <summary>
        /// Permet de sélectionner la station S7-1500 Gateway
        /// </summary>
        /// <returns></returns>
        private bool GetS71500_Gateway_Station(ref string sStationName)
        {
            bool bRet = true;
            string sError = string.Empty;

            sStationName = string.Empty;

            while (true)
            {
                // Test si le projet Tia Portal est déja sélectionné ?
                if (m_oExploreTiaPLCH.GetTiaPortalProjectIsSelected() == false)
                {
                    // Sélection du projet Tia Portal
                    if (m_oExploreTiaPLCH.ChooseTiaProject(ref sError) == false)
                    {
                        bRet = false;
                        break;
                    }
                }
                // Récupération de la station S7-1500 Gateway dans le projet sélectionné
                if (m_oExploreTiaPLCH.GetTiaStationFromProjectList(ref sStationName, ref sError) == false)
                {
                    bRet = false;
                }
                
                break;
            }

            return bRet;
        }

        private void BpStartCodeGenerator_Click(object sender, EventArgs e)
        {
            bool projetSelectionner = m_oExploreTiaPLCH.m_bTiaPortalProjectIsSelected;

            if (projetSelectionner)
            {
                string sErrorText = string.Empty;

                LogInformation_ThreadUIPrincipal(@"Demarrage de la recherche des Blocs FC pour export et analyse");

                while (true)
                {

                    // Lancemement de la lecture des blocs FC  et import du FC dans la CPU  
                    StartPLCCodeGenerator(false);
                    LogInformation_ThreadUIPrincipal(@"Fin de l'import du bloc : FC Aide au diagnostic");
                    LogInformation_ThreadUIPrincipal(@"");
                    LogInformation_ThreadUIPrincipal(@"");
                    break;
                }
            }
            else
            {
                LogInformation_ThreadUIPrincipal(@" ");
                LogInformation_ThreadUIPrincipal(@"                     ATTENTION : Veuillez selectionner un projet et une CPU avant de lancer l'execution du programme");
                LogInformation_ThreadUIPrincipal(@" ");
            }
                    
        }

        /// <summary>
        /// Lancemenent de la génération de code dans la CPU
        /// </summary>
        private void StartPLCCodeGenerator(bool bGenerateOnlyInterface)
        {
            m_bErrorWhenStarted = false;
            // Lancement du processus de génération du code PLC
            TaskEventEndThreadGenerationCode += ThreadGenerationCodeIsFinishedTask;
            m_oThreadStartGeneratePLC = new Thread(() => WaitForEndCodeGeneration());
            try
            {
                m_bCodeGenerationIsStarted = false;
//                StartStopProgressInformation(true);
                TimerRefresh.Enabled = true;

                // Lancement du thread
                m_oThreadStartGeneratePLC.Start();

                BpSelectS71500H.Enabled = false;
                BpStartCodeGenerator.Enabled = false;
                PanelGenerationCode.Visible = true;
                PanelGenerationCode.Refresh();
                PanelGenerationCode.Update();
                Application.DoEvents();

                GeneratePLCCodeMain(bGenerateOnlyInterface);

                while (m_bCodeGenerationIsStarted == false)
                {
                    Application.DoEvents();
                }
            }
            finally
            {
//                StartStopProgressInformation(false);
                TimerRefresh.Enabled = false;

                BpSelectS71500H.Enabled = true;
                PanelGenerationCode.Visible = false;
                BpStartCodeGenerator.Enabled = true;

                // Test si une erreur est apparue pendant la génération
                if (m_bErrorWhenGeneratedCode == true)
                {
                    // On quitte l'application
                    Environment.Exit(0);
                }
                else
                {
                    StatusLabel.Text = "PLC code generation generated correctly";
                    this.Activate();
                }
            }


        }

        /// <summary>
        /// Permet de lancer le raffraichissement de l'interface
        /// </summary>
        private void WaitForEndCodeGeneration()
        {
            while (m_bCodeGenerationIsStarted == false)
            {
                Application.DoEvents();
            }
        }

        /// <summary>
        /// Code principal de génération des codes automate
        /// </summary>
        private void GeneratePLCCodeMain(bool bGenerateOnlyInterface)
        {
            string sErrorText = string.Empty;

            try
            {
                while (true)
                {
                    
                    // Avant le lancement de la génération, il est necessaire de vérifier que le programme dans la Cpu
                    // est bien compilé sans erreur 
                    if (bGenerateOnlyInterface == false)
                    {
                        // Lancement de la compilation du S7-1500
                        LogInformation_ThreadUIPrincipal(@"Compilation de la CPU avant generation...");
                        if (m_oPlcGenerateTiaCode.CompileThisPlcAndCheckErrors(m_oPLC_H_ProjectDefinitions.sPLCS71500HTargetStationName, ref sErrorText) == false)
                        {
                            LogInformation_ThreadUIPrincipal(string.Format(@"Erreur à la compilation de la CPU : {0}", sErrorText));
                            break;
                        }
                        LogInformation_ThreadUIPrincipal(@"La CPU est bien compilée sans erreur");
  
                    }

                    

                    // Lancement de la lecture + Export du code de la CPU S7-1500 
                    
                    if (StartExamineS71500PLCCode(ref sErrorText) == false)
                    {
                        LogInformation_ThreadUIPrincipal(string.Format(@"Erreur à la lecture du code de la CPU : {0}", sErrorText));
                        break;
                    }
                    
                    LogInformation_ThreadUIPrincipal(@"L'export et l'analyse du programme CPU s'est bien déroulé");
 
                    // Lancement de la génération du code dans la CPU S7-1500 
                    if (StartGeneratePLCProgramInPLCS71500(bGenerateOnlyInterface, ref sErrorText) == false)
                    {
                        LogInformation_ThreadUIPrincipal(string.Format(@"Erreur à la génération de code dans la CPU : {0}", sErrorText));
                        break;
                    }

                    LogInformation_ThreadUIPrincipal(@"Le FC a été correctement généré dans la CPU");

                    break;
                }
            }
            finally
            {
                // Indication de fin du thread
                this.BeginInvoke(TaskEventEndThreadGenerationCode, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Evenement de fin de démarrage de l'application
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="arg"></param>
        private void ThreadGenerationCodeIsFinishedTask(object sender, EventArgs arg)
        {
            m_bCodeGenerationIsStarted = true;
        }


        /// <summary>
        /// Lancement de la lecture du code de la CPU S7-1500
        /// </summary>
        /// <param name="sErrorText"></param>
        /// <returns></returns>
        private bool StartExamineS71500PLCCode(ref string sErrorText)
        {
            bool bRet = true;

            sErrorText = string.Empty;

            m_oTiaProjectForCPUH = new TiaProjectForCPUH(m_oExploreTiaPLCH.GetTiainterface().GetTIAPortalProject(),
                                                         PLC_H_Station_Target_Name.Text,
                                                         m_oPLC_H_ProjectDefinitions.sRootFolderBlocOPCUAServer,
                                                         m_oPLC_H_ProjectDefinitions.sRootFolderTagsOPCUAServer,
                                                         m_oPLC_H_ProjectDefinitions.sDBNameMappingOPCUA);

           
            bRet = m_oExploreTiaPLCH.EnumerateFoldersBlocksParametersAndTagsForOPCUA(ref m_oTiaProjectForCPUH, PLC_H_Station_Target_Name.Text, ref sErrorText);


            // Affectation des informations de blocs et tags présents dans le projet TIA du S7-1500 H 
            m_oPlcGenerateTiaCode.SetTiaProjectForCPUH(m_oTiaProjectForCPUH);

            return bRet;
        }


        /// <summary>
        /// Lancement de la génération du code dans l'automate S7-1500 
        /// </summary>
        /// <param name="sErrorText"></param>
        /// <returns></returns>
        private bool StartGeneratePLCProgramInPLCS71500(bool bGenerateOnlyInterface, ref string sErrorText)
        {
            bool bRet = true;
            string sErrorInterne = string.Empty;

            if (bGenerateOnlyInterface == true)
            {
                LogInformation_ThreadUIPrincipal(@"No PLC Code generation for S7-1500 due to choice");
                return bRet;
            }

            LogInformation_ThreadUIPrincipal(@"Lancement de la generation de code dans la CPU");

            while (true)
            {
                bRet = m_oPlcGenerateTiaCode.GenerateTiaCodeForS71500H_R(m_oPLC_H_ProjectDefinitions.sPLCS71500HTargetStationName, ref sErrorInterne);
                if (bRet == false)
                {
                    sErrorText = string.Format(@"Error station {0} : {1}", m_oPLC_H_ProjectDefinitions.sPLCS71500HTargetStationName, sErrorInterne);
                    break;
                }

                break;
            }


            return bRet;
        }

        private void TimerRefresh_Tick(object sender, EventArgs e)
        {
            Gif_Wait.Update();
            Gif_Wait.Refresh();
            this.Invalidate();
            this.Update();
            this.Refresh();
            Application.DoEvents();
        }

       

        //Boutton numero 2 pour import d'un bloc externe
        /*
        private void BpStartInterfaceGenerator_Click(object sender, EventArgs e)
        {
            List<Device> ListDevice = new List<Device>();
            Device oPLCHStation = null;
            PlcSoftware controllertarget = null;
            string sErrorText = string.Empty; ;
            bool bRet = true;

            while (true)
            {

                m_oTiaProjectForCPUH = new TiaProjectForCPUH(m_oExploreTiaPLCH.GetTiainterface().GetTIAPortalProject(),
                                                                PLC_H_Station_Target_Name.Text,
                                                                m_oPLC_H_ProjectDefinitions.sRootFolderBlocOPCUAServer,
                                                                m_oPLC_H_ProjectDefinitions.sRootFolderTagsOPCUAServer,
                                                                m_oPLC_H_ProjectDefinitions.sDBNameMappingOPCUA);


                // Récupération de l'objet station correspondant à l'automate 
                // Lancement de l'énumération des stations dans le projet
                if (m_oExploreTiaPLCH.GetTiainterface().EnumerationDevice(ref ListDevice, ref sErrorText) == false)
                {
                    bRet = false;
                    break;
                }
                // Recherche de la station 
                foreach (Device device in ListDevice)
                {
                    if (device.Name == PLC_H_Station_Target_Name.Text)
                    {
                        oPLCHStation = device;
                        break;
                    }
                }
                // Test si la station a été trouvée ?
                if (oPLCHStation == null)
                {
                    sErrorText = string.Format(@"Station '{0}' for PLC not found !", PLC_H_Station_Target_Name.Text);
                    bRet = false;
                    break;
                }

                // Récupération du controllertarget
                DeviceItem deviceItemToGetService = m_oExploreTiaPLCH.TryToFoundDeviceItemInDevice(oPLCHStation);
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

                string nomSource = "LTU_Demo";
                string cheminSource = @"C:\Users\SIEMENS\Desktop\HMAOPCUAH\LTU_Demo.scl";

                //Suppression de l'ancienne source externe SCL
                controllertarget.ExternalSourceGroup.ExternalSources.Find(nomSource).Delete();

                //Import du FC block source SCL
                m_oExploreTiaPLCH.ImportSourceFileToSourceFolder(controllertarget.ExternalSourceGroup, nomSource, cheminSource, ref sErrorText);

                //Conversion et compilation du FC block  
                controllertarget.ExternalSourceGroup.ExternalSources.Find(nomSource).GenerateBlocksFromSource();
              

                break;
            }
        }*/
    }
}
