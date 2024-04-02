using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Schema;

namespace GlobalsOPCUAH
{
  
    /// <summary>
    /// Classe de définition d'une variable dans Tia Portal
    /// </summary>
    public class TiaPortalVariable
    {
        // Nom de la variable
        public string Name { get; set; }
        // Type de la variable
        public string Type { get; set; }
        // Type de la variable bute
        public string RawType { get; set; }
        // Bloc parent s'il existe
        public TiaPortalBloc BlocParent { get; set; }
        // Identifiant unique de la variable
        public int Id { get; set; }

        // Noeud folder parent
        public TreeNode Parentnode { get; set; }

        // Définition du commentaire sur la variable (Description dans le fichier OPC UA)
        public string Commentar { get; set; }

        // Définition de l'adresse de variable de mapping
        public string MappingVariable { get; set; }

        // Définition de la variable en ReadOnly
        public bool ReadOnly { get; set; }

        // Définition du chemin de variables couurant
        public string CurrentPathVariable { get; set; }

        /// <summary>
        /// Constructeur de la classe 
        /// </summary>
        /// <param name="sName"> Nom de la variable </param>
        /// <param name="blocparent"> Nom du bloc parent dans le cas d'un DB </param>
        /// <param name="folderparent"> Folder parent dans le cas de variables de type tags </param>
        public TiaPortalVariable(string sName, TiaPortalBloc blocparent, string type, string rawtype, int iId, TreeNode nodeparent, 
                                 string sCommentar, string sCurrentPathVariable)
        {
            Name = sName;
            BlocParent = blocparent;
            Type = type;
            RawType = rawtype;
            Id = iId;
            Parentnode = nodeparent;
            Commentar = sCommentar;
            MappingVariable = string.Empty;
            ReadOnly = false;
            CurrentPathVariable = sCurrentPathVariable;
        }

        /// <summary>
        /// Constructeur de la classe évolué pour les variables system H
        /// </summary>
        /// <param name="sName"> Nom de la variable </param>
        /// <param name="blocparent"> Nom du bloc parent dans le cas d'un DB </param>
        /// <param name="folderparent"> Folder parent dans le cas de variables de type tags </param>
        public TiaPortalVariable(string sName, TiaPortalBloc blocparent, string type, string rawtype, int iId, TreeNode nodeparent, string sCommentar, 
                                 string sMappingVariable, bool bReadOnly, string sCurrentPathVariable)
        {
            Name = sName;
            BlocParent = blocparent;
            Type = type;
            RawType = rawtype;
            Id = iId;
            Parentnode = nodeparent;
            Commentar = sCommentar;
            MappingVariable = sMappingVariable;
            ReadOnly = bReadOnly;
            CurrentPathVariable = sCurrentPathVariable;
        }

    }


    /// <summary>
    /// Classe de définition d'un bloc DB dans Tia Portal
    /// </summary>
    public class TiaPortalBloc
    {
        // Nom du bloc
        public string Name { get; set; }
        // Liste des variables définies dans le bloc
        public List<TiaPortalVariable> VariablesList { get; set; }
        // Node folder reference
        public TreeNode Parentnode { get; set; }
        // Identifiant unique du folder
        public int Id { get; set; }
        // Nouveau nom dans le cas d'une regénération
        public string NewName { get; set; }

        /// <summary>
        /// Constructeur de la classe
        /// </summary>
        /// <param name="sBlocName"> Nom du bloc </param>
        /// <param name="parentnode"> Folder node parent </param>
        /// <param name="parentfullpath"> Chemin complet pour ce noeud </param>
        public TiaPortalBloc(string sBlocName, TreeNode parentnode, int iId, string sNewName)
        {
            Name = sBlocName;
            Parentnode = parentnode;
            VariablesList = new List<TiaPortalVariable>();
            Id = iId;
            NewName = sNewName;
        }
    }


    /// <summary>
    /// Classe de définition d'un répertoire dans Tia Portal
    /// </summary>
    public class TiaPortalFolder
    {
        // Nom du folder
        public string Name { get; set; }
        // Chemin complet du folder Tia Portal
        public string Path { get; set; }
        // Identifiant unique du folder
        public int Id { get; set; }

        /// <summary>
        /// Constructeur de la classe
        /// </summary>
        /// <param name="sName"> Nom du folder </param>
        public TiaPortalFolder(string sName, int iId)
        {
            Name = sName;
            Id = iId;
        }
    }

    /// <summary>
    /// Classe de définition du projet référence de la Cpu H
    /// </summary>
    public class TiaProjectForCPUH
    {
        #region Constantes

        // Valeur du premier identifiant pour les folders
        private const int m_iStartFolderId = 1200;

        // Chaine séparateur pour le nom du chemin complet d'un folder
        private const string m_sSeparatorPath = @"\";

        // HMA 28/04/2022
     //   private const string m_sDummyFolderRootWhenFolderNotSpecify = @"HMAFolderReplaceWhenRootNotSpecify";

        #endregion


        #region Variables

        // Nom du projet TIA Portal cible pour l'automate passerelle
        public string sProjectName { get; set; }
        // Nom de la station cible pour l'automate passerelle
        public string sStationName { get; set; }
        // Nom de la racine pour les blocs
        public string sRootBlocsName { get; set; }
        // Nom de la racine pour les tags
        public string sRootTagsName { get; set; }

        // Noeud arborescence pour les folders pour les blocs
        private TreeNode m_oRootNodefolderBlocks;
        public TreeNode GetRootNodefolderBlocks() { return m_oRootNodefolderBlocks; }

        // Noeud arborescence pour les folders pour les tags
        private TreeNode m_oRootNodefolderTags;
        public TreeNode GetRootNodefolderTags() { return m_oRootNodefolderTags; }

        // Noeud arborescence pour les folders pour les variables system H
        private TreeNode m_oRootNodefolderHSystemTags;
        public TreeNode GetRootNodefolderHSystemTags() { return m_oRootNodefolderHSystemTags; }

        // Liste des blocs Tia Portal du projet
        public List<TiaPortalBloc> oListblocs { get; set; } = new List<TiaPortalBloc>();

        // Liste des variables sous le folder Tags
        public List<TiaPortalVariable> oListTagsVariables { get; set; } = new List<TiaPortalVariable>();

        // Liste des variables sous le folder des variables systeme H
        public List<TiaPortalVariable> oListSystemHTagsVariables { get; set; } = new List<TiaPortalVariable>();

        // Dernier identifiant disponible pour affectation dans les folders et variables
        private int m_iLastFolderVariableId;
        
        // Nom du controlleur automate h dans le projet le source
        public string sPLCHControllerTargetName { get; set; }

        // Nom du DB pour le mapping OPC UA
        public string m_sDBNameMappingOPCUA { get; set; }

        // Nom du fichier XML généré
        public string m_sXmlFileOPCUAInterface { get; set; }

        // Répertoire par défaut pour le system H
        public string m_sRootVariableSystemName = @"System";

        // Répertoire par défaut pour les variables system H
        public string m_sRootVariableSystemHName = @"Variables_PLC_H";

        #endregion

        /// <summary>
        /// Constructeur par defaut de la classe
        /// </summary>
        public TiaProjectForCPUH()
        {
            m_iLastFolderVariableId = m_iStartFolderId;
        }

        /// <summary>
        /// Constructeur de la classe
        /// </summary>
        /// <param name="sprojectName"> Nom du projet Tia Portal </param>
        /// <param name="sstationname"> Nom de la station cible Gateway </param>
        /// <param name="srootblocsname"> Nom de la root dans l'arborescence du serveur OPC UA pour les blocs </param>
        /// <param name="sroottagsname"> Nom de la root dans l'arborescence du serveur OPC UA pour les tags </param>
        /// <param name="sDBNameMappingOPCUA"> Nom du DB de mapping OPC UA </param>
        public TiaProjectForCPUH(string sprojectName, string sstationname, string srootblocsname, string sroottagsname,
                                 string sDBNameMappingOPCUA) : this()
        {
            sProjectName = sprojectName;
            sStationName = sstationname;
            sRootBlocsName = srootblocsname;
            sRootTagsName = sroottagsname;
            m_sDBNameMappingOPCUA = sDBNameMappingOPCUA;

            // Ajout d'un folder root par defaut s'il l'on en défini un
            if (sRootBlocsName.Length != 0)
            {
                m_oRootNodefolderBlocks = new TreeNode(sRootBlocsName);
                m_oRootNodefolderBlocks.Tag = new TiaPortalFolder(sRootBlocsName,
                                                                  GetNextFolderVariableId());
            }
            else
            {
                // HMA 28/04/2022
/*
                m_oRootNodefolderBlocks = new TreeNode(m_sDummyFolderRootWhenFolderNotSpecify);
                m_oRootNodefolderBlocks.Tag = new TiaPortalFolder(m_sDummyFolderRootWhenFolderNotSpecify,
                                                                  GetNextFolderVariableId());
*/
            }
            if (sRootTagsName.Length != 0)
            {
                m_oRootNodefolderTags = new TreeNode(sRootTagsName);
                m_oRootNodefolderTags.Tag = new TiaPortalFolder(sRootTagsName,
                                                                GetNextFolderVariableId());
            }
            // Ajout du noeud liste des variables system pour le H
            m_oRootNodefolderHSystemTags = new TreeNode(m_sRootVariableSystemName);
            m_oRootNodefolderHSystemTags.Tag = new TiaPortalFolder(m_sRootVariableSystemName,
                                                                   GetNextFolderVariableId());
        }

/*
        /// <summary>
        /// Permet de tester si l'on se trouve sur la racine de remplacement
        /// </summary>
        /// <param name="sRootFolder"></param>
        /// <returns></returns>
        private bool CheckIfDummyFolderRootWhenFolderNotSpecify(string sRootFolder)
        {
            bool bRet = false;

            if (sRootFolder.ToUpper() == m_sDummyFolderRootWhenFolderNotSpecify.ToUpper())
            {
                bRet = true;
            }
            else bRet = false;

            return bRet;
        }
*/

        /// <summary>
        /// Permet de calculer et retourner le prochain identifiant
        /// </summary>
        /// <returns></returns>
        public int GetNextFolderVariableId()
        {
            int iRet = m_iLastFolderVariableId;

            m_iLastFolderVariableId++;

            return iRet;
        }
    }


    /// <summary>
    /// Permettant de définir un tableau d'élément
    /// </summary>
    public class TiaArrayOfElement
    {
        #region Variables

        private string m_sArrayDefinition;                    // Définition du tableau

        private int m_iMinArray = 1;                          // Base mini du tableau
        public int GetMinArray() { return m_iMinArray; }
        private int m_iMaxArray = 2;                          // Base maxi du tableau
        public int GetMaxArray() { return m_iMaxArray; }

        private int m_iNumberOfElement;                       // Nombre d'élements dans le tableau
        public int GetNumberOfElement() { return m_iNumberOfElement; }

        private string m_sArrayTypeName;                      // Type d'élément du tableau
        public string GetArrayOfTypeName() { return m_sArrayTypeName; }

        private string m_sArrayName;                          // nom du tableau
        public string GetArrayName() { return m_sArrayName; }

        #endregion

        /// <summary>
        /// Constructeur de base de la classe
        /// </summary>
        public TiaArrayOfElement(string sArrayDefinition, string sArrayName)
        {
            m_sArrayDefinition = sArrayDefinition;
            m_sArrayName = sArrayName;
            DecodeArray();
        }

        /// <summary>
        /// Permet de décoder la définition du tableau
        /// </summary>
        private void DecodeArray()
        {
            // Récupération des indices de tableau
            string sIndicesArray = m_sArrayDefinition.Substring(m_sArrayDefinition.IndexOf('[') + 1, m_sArrayDefinition.IndexOf(']') - m_sArrayDefinition.IndexOf('[') - 1);
            m_iMinArray = int.Parse(sIndicesArray.Substring(0, sIndicesArray.IndexOf("_TO_")));
            int iPosEndMarck = sIndicesArray.IndexOf("_TO_") + "_TO_".Length;
            m_iMaxArray = int.Parse(sIndicesArray.Substring(iPosEndMarck, sIndicesArray.Length - iPosEndMarck));
            m_iNumberOfElement = m_iMaxArray - m_iMinArray + 1;
            m_sArrayTypeName = m_sArrayDefinition.Substring(m_sArrayDefinition.IndexOf(" of ") + " of ".Length, 
                                                            m_sArrayDefinition.Length - (m_sArrayDefinition.IndexOf(" of ") + " of ".Length + 1));
        }
    }

    /// <summary>
    /// Classe liste des tableau d'élements
    /// </summary>
    public class ListOfTiaArrayOfElement
    {
        private List<TiaArrayOfElement> m_oListOfElement = new List<TiaArrayOfElement>();

        public List<TiaArrayOfElement> GetList() { return m_oListOfElement; }

        /// <summary>
        /// Permet de générer la liste pour des tableaux imbriqués
        /// </summary>
        /// <returns></returns>
        public List<string> GetListOfVariablePath()
        {
            List<string> oListPath = new List<string>();
            List<string> oListOfPreviousarrays = new List<string>();

            // Boucle de traitement de tous les tableaux d'éléments
            foreach (TiaArrayOfElement tiaarrayelement in m_oListOfElement)
            {
                oListOfPreviousarrays = GetPathFromThisArray(tiaarrayelement, oListOfPreviousarrays);
            }


            return oListPath;
        }

        private List<string> GetPathFromThisArray(TiaArrayOfElement tiaarrayelement, List<string> oListOfPreviousarrays)
        {
            List<string> olist = new List<string>();
            string sElementPath = string.Empty;

            // Génération des items pour ce tableau
            for(int i= tiaarrayelement.GetMinArray(); i <= tiaarrayelement.GetMaxArray(); i++)
            {
                // Test si premier niveau de tableau ?
                if (oListOfPreviousarrays.Count == 0)
                {
                    sElementPath = string.Format(@"{0}[{1}]", tiaarrayelement.GetArrayName(), i);
                    olist.Add(sElementPath);
                }
                else
                {
                    foreach(string sRootElement in oListOfPreviousarrays)
                    {


                    }
                }

            }

            return olist;
        }


    }

}
