﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Odbc;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Capa_Vista_Transaccion_Proveedor
{
    public partial class Frm_transaccion_proveedores : Form
    {
        Capa_Controlador_Transaccion_Proveedor.Controlador controlador = new Capa_Controlador_Transaccion_Proveedor.Controlador();
        public Frm_transaccion_proveedores()
        {
            InitializeComponent();
        }

        private void Btn_editar_Click(object sender, EventArgs e)
        {
            if (Dgv_transacciones_provee.SelectedRows.Count > 0)
            {
                // Obtener los datos de la fila seleccionada
                Txt_transaccion.Text = Dgv_transacciones_provee.CurrentRow.Cells["Pk_id_transaccion"].Value.ToString();
                Txt_proveedor.Text = Dgv_transacciones_provee.CurrentRow.Cells["Fk_id_proveedor"].Value.ToString();
                Txt_fecha.Text = Dgv_transacciones_provee.CurrentRow.Cells["fecha_transaccion"].Value.ToString();
                Cbo_tipotransacci.Text = Dgv_transacciones_provee.CurrentRow.Cells["transaccion_tipo"].Value.ToString();
                Txt_monto.Text = Dgv_transacciones_provee.CurrentRow.Cells["transaccion_monto"].Value.ToString();
                Cbo_moneda.Text = Dgv_transacciones_provee.CurrentRow.Cells["transaccion_tipo_moneda"].Value.ToString();
                Cbo_estado.Text = Dgv_transacciones_provee.CurrentRow.Cells["transaccion_estado"].Value.ToString();
                Txt_cuenta.Text = Dgv_transacciones_provee.CurrentRow.Cells["Fk_id_transC"].Value.ToString();

                // Activar modo edición
                controlador.esEdicion = true;
                controlador.idTransaccionSeleccionada = Txt_transaccion.Text;
            }
            else
            {
                MessageBox.Show("Seleccione una fila para editar", "Advertencia",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void Btn_guardar_Click(object sender, EventArgs e)
        {
            int resultado = controlador.GuardarTransaccion(Txt_transaccion, Txt_proveedor,
                                                           Txt_fecha.Text, Cbo_tipotransacci.Text,
                                                           Txt_monto.Text, Cbo_moneda.Text,
                                                            Cbo_estado.Text, Txt_cuenta);

            if (resultado == 1)
            {
                CargarDatos();
                limpiarCampos();
            }
        }
        private void CargarDatos()
        {
            try
            {
                OdbcDataAdapter adapter = controlador.DisplayTransaccion();
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                Dgv_transacciones_provee.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar los datos: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void Transaccion_proveedores_Load(object sender, EventArgs e)
        {
            CargarDatos();
        }

        private void Btn_eliminar_Click(object sender, EventArgs e)
        {
            if (Dgv_transacciones_provee.SelectedRows.Count > 0)
            {
                string idTransaccion = Dgv_transacciones_provee.CurrentRow.Cells["Pk_id_transaccion"].Value.ToString();

                var confirmResult = MessageBox.Show("¿Estás seguro de que deseas eliminar este registro?",
                                                    "Confirmar eliminación",
                                                    MessageBoxButtons.YesNo,
                                                    MessageBoxIcon.Warning);

                if (confirmResult == DialogResult.Yes)
                {
                    controlador.eliminarTransaccion(idTransaccion);
                    MessageBox.Show("Registro eliminado correctamente.", "Éxito",
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Actualizar DataGridView
                    CargarDatos();
                }
            }
            else
            {
                MessageBox.Show("Seleccione un registro para eliminar.",
                                "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        private void limpiarCampos()
        {
            // Limpiar todos los TextBox
            Txt_transaccion.Clear();
            Txt_proveedor.Clear();
            Txt_fecha.Clear();
            Txt_monto.Clear();
            Txt_cuenta.Clear();

            // Restablecer el ComboBox a su estado inicial
            Cbo_estado.SelectedIndex = -1;
            Cbo_moneda.SelectedIndex = -1;
            Cbo_tipotransacci.SelectedIndex = -1;

            // Opcional: Quitar la selección del DataGridView
            Dgv_transacciones_provee.ClearSelection();
        }
        private string determinarCampoDeBusqueda()
        {
            if (!string.IsNullOrEmpty(Txt_transaccion.Text))
                return "Pk_id_transaccion";
            else if (!string.IsNullOrEmpty(Txt_proveedor.Text))
                return "Fk_id_proveedor";
            else if (!string.IsNullOrEmpty(Txt_cuenta.Text))
                return "Fk_id_transC";
            else
                return null;
        }

        private string obtenerValorDeBusqueda()
        {
            if (!string.IsNullOrEmpty(Txt_transaccion.Text))
                return Txt_transaccion.Text;
            else if (!string.IsNullOrEmpty(Txt_proveedor.Text))
                return Txt_proveedor.Text;
            else if (!string.IsNullOrEmpty(Txt_cuenta.Text))
                return Txt_cuenta.Text;
            else
                return null;
        }
        private void Btn_buscar_Click(object sender, EventArgs e)
        {
            string campo = determinarCampoDeBusqueda();
            string valor = obtenerValorDeBusqueda();

            if (!string.IsNullOrEmpty(campo) && !string.IsNullOrEmpty(valor))
            {
                OdbcDataAdapter da = controlador.buscarTransaccionPorCampo(campo, valor);
                DataTable dt = new DataTable();
                da.Fill(dt);
                Dgv_transacciones_provee.DataSource = dt;
            }
            else
            {
                MessageBox.Show("Ingrese un valor en uno de los campos de ID para buscar.",
                                "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void Btn_Salir_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Btn_ayuda_Click(object sender, EventArgs e)
        {
            try
            {
                // Obtener la ruta del directorio del ejecutable
                string sexecutablePath = AppDomain.CurrentDomain.BaseDirectory;

                // Retroceder a la carpeta del proyecto
                string sprojectPath = Path.GetFullPath(Path.Combine(sexecutablePath, @"..\..\"));
                //MessageBox.Show("1" + projectPath);


                string sayudaFolderPath = Path.Combine(sprojectPath, "AyudaCuentasCorrientes");


                // Busca el archivo .chm en la carpeta "Ayuda_Seguridad"
                string spathAyuda = FindFileInDirectory(sayudaFolderPath, "AyudaTransaccProvee.chm");

                // Verifica si el archivo existe antes de intentar abrirlo
                if (!string.IsNullOrEmpty(spathAyuda))
                {
                    // Abre el archivo de ayuda .chm en la sección especificada
                    Help.ShowHelp(null, spathAyuda, "TransaccionProveedores.html");
                }
                else
                {
                    // Si el archivo no existe, muestra un mensaje de error
                    MessageBox.Show("El archivo de ayuda no se encontró.");
                }
            }
            catch (Exception ex)
            {
                // Mostrar un mensaje de error en caso de una excepción
                MessageBox.Show("Ocurrió un error al abrir la ayuda: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Console.WriteLine("Error al abrir la ayuda: " + ex.ToString());
            }
        }

        public string FindFileInDirectory(string sdirectory, string sfileName)
        {
            try
            {
                // Verificamos si la carpeta existe
                if (Directory.Exists(sdirectory))
                {
                    // Buscamos el archivo .chm en la carpeta
                    string[] files = Directory.GetFiles(sdirectory, "*.chm", SearchOption.TopDirectoryOnly);

                    // Si encontramos el archivo, verificamos si coincide con el archivo que se busca y retornamos su ruta
                    foreach (var file in files)
                    {
                        if (Path.GetFileName(file).Equals(sfileName, StringComparison.OrdinalIgnoreCase))
                        {
                            //MessageBox.Show("Archivo encontrado: " + file);
                            return file;
                        }
                    }
                }
                else
                {
                    MessageBox.Show("No se encontró la carpeta: " + sdirectory);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al buscar ayuda: " + ex.Message);
            }

            // Retorna null si no se encontró el archivo
            return null;
        }

        private void Btn_reporte_Click(object sender, EventArgs e)
        {
            Rpt_TransaccionProveedores
            frm = new Rpt_TransaccionProveedores();
            frm.Show();
        }
    }
}
