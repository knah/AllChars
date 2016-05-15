using System;
using System.Drawing;
using System.Windows.Forms;

namespace AllCharsNS {
    public partial class ComposeHelp : Form {
        private Configuration config;

        public ComposeHelp(Configuration config) {
            this.config = config;

            InitializeComponent();

            fontChooser.Items.Clear();
            foreach (FontFamily ff in FontFamily.Families)
                if (ff.IsStyleAvailable(FontStyle.Regular)) {
                    fontChooser.Items.Add(ff);
                }

            FontFamily chosenFont;
            try
            {
                chosenFont = new FontFamily("Arial Unicode MS");
                if (!chosenFont.IsStyleAvailable(FontStyle.Regular))
                    throw new ArgumentException();
            }
            catch (ArgumentException) {
                try {
                    chosenFont = new FontFamily("Lucida Sans Unicode");
                    if (!chosenFont.IsStyleAvailable(FontStyle.Regular))
                        throw new ArgumentException();
                }
                catch (ArgumentException) {
                    chosenFont = new FontFamily("Arial");                    
                }
            }

            fontChooser.SelectedItem = chosenFont;
        }

        private void ComposeHelp_Load(object sender, EventArgs e) {
            categoryList.Items.Clear();
            categoryList.Items.AddRange(config.ComposeCategories);
        }

        private void closeButton_Click(object sender, EventArgs e) {
            Close();
        }

        private void categoryList_SelectedIndexChanged(object sender, EventArgs e) {
            composeGrid.DataSource = config.GetSequencesByCategory((string)categoryList.SelectedItem);
        }

        private void fontChooser_SelectedIndexChanged(object sender, EventArgs e) {
            DataGridViewCellStyle charStyle = composeGrid.Columns["characterColumn"].DefaultCellStyle;
            charStyle.Font =
                new Font((FontFamily)fontChooser.SelectedItem, charStyle.Font.Size, FontStyle.Regular,
                         charStyle.Font.Unit);
        }
    }
}