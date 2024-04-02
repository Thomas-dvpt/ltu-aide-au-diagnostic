using Siemens.Engineering;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TiaExplorePLCH
{
    /// <summary>
    /// Boite de dialogue permettant de sélectionner un projet Tia Portal
    /// </summary>
    public partial class TiaPortalProjectSelection : Form
    {
        #region Enumeration

        public enum TiaProjectSelectionType : int { NoSelectionType = 0, CurrentTiaProject = 1, NewTiaProject = 2}

        #endregion

        #region Variables

        // Objet dictionnaire de la liste des projets Tia Portal en cours de traitement
        public Dictionary<string, HMATiaPortalProcess> m_oDictionnaryTiaProcessList = new Dictionary<string, HMATiaPortalProcess>();

        // Permet d'indiquer si un projet a été sélectionné
        public bool bOneProjectSelected = false;

        // Type de sélection du projet
        public TiaProjectSelectionType iTiaProjectSelectType = TiaProjectSelectionType.NoSelectionType;

        // Chemin complet du nouveau Tia Portal
        public string sNewTiaPortalSelectionPath = string.Empty;

        #endregion

        public TiaPortalProjectSelection()
        {
            InitializeComponent();
        }

        private void TiaPortalProjectSelection_Load(object sender, EventArgs e)
        {
            // Affichage des informations dans ma boite de sélection

            // Test si des instances sont en cours de traitement ?
            if (m_oDictionnaryTiaProcessList.Count != 0)
            {
                // Remplissage de la Combobox de sélection
                foreach (var tiaportalprocess in m_oDictionnaryTiaProcessList)
                {
                    CurrentProjectsSelection.Items.Add(tiaportalprocess.Value);
                }
                // On sélectionne le premier choix
                CurrentProjectsSelection.SelectedIndex = 0;
                CurrentProjectsSelection.Enabled = true;
                BpSelectCurrentProject.Enabled = true;
            }
            else
            {
                BpSelectCurrentProject.Enabled = false;
            }
        }

        /// <summary>
        /// Permet de sélectionner le projet en cours de traitement sélectionné
        /// </summary>
        /// <returns></returns>
        public HMATiaPortalProcess GetSelectCurrentProject()
        {
            HMATiaPortalProcess hmatiaportalprocess = null;

            hmatiaportalprocess = (HMATiaPortalProcess)(CurrentProjectsSelection.Items[CurrentProjectsSelection.SelectedIndex]);

            //YOU PROJET EN COURS

            return hmatiaportalprocess;
        }

        private void BpSelectCurrentProject_Click(object sender, EventArgs e)
        {
            // On a validé un projet actuellement ouvert
            bOneProjectSelected = true;
            iTiaProjectSelectType = TiaProjectSelectionType.CurrentTiaProject;
            Close();
        }

        private void BpValidSelectProject_Click(object sender, EventArgs e)
        {
            // On a validé un projet nouvellement sélectionné
            bOneProjectSelected = true;
            iTiaProjectSelectType = TiaProjectSelectionType.NewTiaProject;
            Close();
        }

        /*
        private void BpSelectProject_Click(object sender, EventArgs e)
        {
            // Sélection du fichier projet TIA Portal
            if (openFileDialogTIAPortalProject.ShowDialog() == DialogResult.OK)
            {
                sNewTiaPortalSelectionPath = openFileDialogTIAPortalProject.FileName;
                NewTiaProject.Text = sNewTiaPortalSelectionPath;
            }

        }*/
    }
}
