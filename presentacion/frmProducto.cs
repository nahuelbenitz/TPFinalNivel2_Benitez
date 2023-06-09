using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using dominio;
using negocio;

namespace presentacion
{
    public partial class frmProducto : Form
    {
        private List<Articulo> listaArticulo;
        public frmProducto()
        {
            InitializeComponent();
        }

        //EVENTOS
        
        private void frmProducto_Load(object sender, EventArgs e)
        {
            cargar();
            cboCampo.Items.Add("Nombre");
            cboCampo.Items.Add("Marca");
            cboCampo.Items.Add("Categoria");
            cboCampo.Items.Add("Precio");
            dgvArticulo.Rows[0].Selected = false;
        }

        private void cboCampo_SelectedIndexChanged(object sender, EventArgs e)
        {
            string opcion = cboCampo.SelectedItem.ToString();
            if (opcion == "Precio")
            {
                cboCriterio.Items.Clear();
                cboCriterio.Items.Add("Mayor a");
                cboCriterio.Items.Add("Menor a");
                cboCriterio.Items.Add("Igual a");
            }
            else
            {
                cboCriterio.Items.Clear();
                cboCriterio.Items.Add("Comienza con");
                cboCriterio.Items.Add("Termina con");
                cboCriterio.Items.Add("Contiene");
            }
        }
        
        private void dgvArticulo_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvArticulo.CurrentRow != null)
            {
                Articulo seleccionado = (Articulo)dgvArticulo.CurrentRow.DataBoundItem;
                cargarImagen(seleccionado.UrlImagen);
            }
        }       

        //EVENTOS DE BOTONES
        
        private void btnAgregar_Click(object sender, EventArgs e)
        {
            frmAltaProducto alta = new frmAltaProducto();
            alta.ShowDialog();
            cargar();
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            try
            {
                Articulo seleccionado;
                if (dgvArticulo.SelectedRows.Count > 0) //me aseguro que haya una fila seleccionada
                {
                    seleccionado = (Articulo)dgvArticulo.CurrentRow.DataBoundItem;

                    frmAltaProducto modificar = new frmAltaProducto(seleccionado);
                    modificar.ShowDialog();
                    cargar();
                }
                else
                    MessageBox.Show("Por favor, seleccione un articulo", "¡Atención!", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
            catch (Exception ex)
            {
                MessageBox.Show("Por favor, seleccione un articulo", "¡Atención!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
        }

        private void btnVerDetalle_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvArticulo.SelectedRows.Count > 0)
                    verDetalle();
                else
                    MessageBox.Show("Por favor, seleccione un articulo", "¡Atención!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Por favor, seleccione un articulo", "¡Atención!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
  
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (dgvArticulo.SelectedRows.Count > 0)
                eliminar();
            else
                MessageBox.Show("Por favor, seleccione un articulo", "¡Atención!", MessageBoxButtons.OK, MessageBoxIcon.Error);

        }

        private void btnFiltrar_Click(object sender, EventArgs e)
        {
            ArticuloNegocio negocio = new ArticuloNegocio();
            try
            {
                if (validarFiltro())
                    return;

                string campo = cboCampo.SelectedItem.ToString();
                string criterio = cboCriterio.SelectedItem.ToString();
                string filtro = txtFiltro.Text;
                dgvArticulo.DataSource = negocio.filtrar(campo, criterio, filtro);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

        }

        //FUNCIONES

        private void cargar()
        {
            ArticuloNegocio negocio = new ArticuloNegocio();
            try
            {
                listaArticulo = negocio.listar();
                dgvArticulo.DataSource = listaArticulo;
                ocultarColumnas();
                cargarImagen(listaArticulo[0].UrlImagen);
                dgvArticulo.Columns["Precio"].DefaultCellStyle.Format = "0.00"; //formateo la columna precio
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void cargarImagen(string imagen)
        {
            try
            {
                pbxArticulo.Load(imagen);
            }
            catch (Exception ex)
            {
                pbxArticulo.Load("https://efectocolibri.com/wp-content/uploads/2021/01/placeholder.png");
            }
        }

        private void ocultarColumnas()
        {
            dgvArticulo.Columns["UrlImagen"].Visible = false;
            dgvArticulo.Columns["Id"].Visible = false;
        }

        private void eliminar(bool logico = false)
        {
            ArticuloNegocio negocio = new ArticuloNegocio();
            Articulo seleccionado;
            try
            {
                    DialogResult respuesta = MessageBox.Show("¿Seguro que querés eliminarlo?", "Eliminando", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (respuesta == DialogResult.Yes)
                    {
                        seleccionado = (Articulo)dgvArticulo.CurrentRow.DataBoundItem;

                        negocio.eliminar(seleccionado.Id);

                        cargar();
                    }
                                    
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                
            }
        }

        private void verDetalle()
        {
            try
            {
                String Codigo = dgvArticulo.CurrentRow.Cells["Codigo"].Value.ToString();                     
                String Nombre = dgvArticulo.CurrentRow.Cells["Nombre"].Value.ToString();
                String Descripcion = dgvArticulo.CurrentRow.Cells["Descripcion"].Value.ToString();
                Descripcion = Descripcion != "" ? Descripcion : "(vacio)"; //por si no completan la descripcion, los demas son obligatorios, no necesitan validar
                String Marca = dgvArticulo.CurrentRow.Cells["Marca"].Value.ToString();
                String Categoria = dgvArticulo.CurrentRow.Cells["Categoria"].Value.ToString();
                decimal precio = Convert.ToDecimal(dgvArticulo.CurrentRow.Cells["Precio"].Value);
                precio = Math.Round(precio, 2);
                string precioFormateado = precio.ToString("0.00"); //muestro el precio formateado a 2 decimales

                MessageBox.Show("El codigo de su producto es: " + Codigo + "\r\nEl nombre es: " + Nombre + "\r\nSu descripcion es: " + Descripcion + "\r\nSu marca es: " + Marca + "\r\nSu categoria es: " + Categoria + "\r\nSale: " + precioFormateado + "$", "Detalle del producto", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());
            }
                    
        }

        private bool validarFiltro()
        {
            if (cboCampo.SelectedIndex < 0)
            {
                MessageBox.Show("Por favor, seleccione el campo para filtrar.", "¡Atención!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return true;
            }
            if (cboCriterio.SelectedIndex < 0)
            {
                MessageBox.Show("Por favor, seleccione el criterio para filtrar.", "¡Atención!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return true;
            }
            if (cboCampo.SelectedItem.ToString() == "Precio")
            {
                if (string.IsNullOrEmpty(txtFiltro.Text))
                {
                    MessageBox.Show("Por favor, ingrese el precio a filtrar", "¡Atención!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return true;
                }
                if (!(soloNumeros(txtFiltro.Text)))
                {
                    MessageBox.Show("Por favor, solo numeros", "¡Atención!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return true;
                }
            }

            return false;
        }

        private bool soloNumeros(string cadena)
        {
            foreach (char caracter in cadena)
            {
                if (!(char.IsNumber(caracter)))
                    return false;
            }
            return true;
        }

        
    }
}
