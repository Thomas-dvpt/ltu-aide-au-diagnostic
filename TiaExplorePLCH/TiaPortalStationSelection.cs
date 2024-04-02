using Siemens.Engineering.HW;
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
    /// Boite de dialogue permettant de sélectionner une station
    /// dans un projet Tia Portal
    /// </summary>
    public partial class TiaPortalStationSelection : Form
    {
        #region Variables

        // Objet dictionnaire de la liste des stations Tia Portal dans le projet en cours de traitement
        public Dictionary<string, HMATiaPortalDevice> m_oDictionnaryTiaStationList = new Dictionary<string, HMATiaPortalDevice>();

        // Permet d'indiquer si un projet a été sélectionné
        public bool bOneStationSelected = false;

        #endregion

        public TiaPortalStationSelection()
        {
            InitializeComponent();
        }


        private void TiaPortalStationSelection_Load(object sender, EventArgs e)
        {
            // Chargement de la liste des stations
            // Test si des instances sont en cours de traitement ?
            if (m_oDictionnaryTiaStationList.Count != 0)
            {
                // Remplissage de la Combobox de sélection
                foreach (var device in m_oDictionnaryTiaStationList)
                {
                    CurrentStationsSelection.Items.Add(device.Value);
                }
                // On sélectionne le premier choix
                CurrentStationsSelection.SelectedIndex = 0;
                CurrentStationsSelection.Enabled = true;
                BpSelectCurrentStation.Enabled = true;
            }
            else
            {
                BpSelectCurrentStation.Enabled = false;
            }

        }

        private void BpSelectCurrentStation_Click(object sender, EventArgs e)
        {
            bOneStationSelected = true;
            Close();
        }

        /// <summary>
        /// Permet de sélectionner la station en cours de traitement sélectionnée
        /// </summary>
        /// <returns></returns>
        public HMATiaPortalDevice GetSelectCurrentStation()
        {
            HMATiaPortalDevice device = null;

            device = (HMATiaPortalDevice)(CurrentStationsSelection.Items[CurrentStationsSelection.SelectedIndex]);

            return device;
        }

    }
}
