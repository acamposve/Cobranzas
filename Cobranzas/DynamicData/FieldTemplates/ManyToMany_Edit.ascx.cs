using System;
using System.Data;
using System.Data.Objects;
using System.Data.Objects.DataClasses;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Collections;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Xml.Linq;
using System.Web.DynamicData;

namespace Cobranzas
{
    public partial class ManyToMany_EditField : System.Web.DynamicData.FieldTemplateUserControl
    {
        public void Page_Load(object sender, EventArgs e)
        {
            // Registrar el evento de actualización de DataSource
            EntityDataSource ds = (EntityDataSource)this.FindDataSourceControl();

            // Esta plantilla de campo se usa para la edición y para la inserción
            ds.Updating += new EventHandler<EntityDataSourceChangingEventArgs>(DataSource_UpdatingOrInserting);
            ds.Inserting += new EventHandler<EntityDataSourceChangingEventArgs>(DataSource_UpdatingOrInserting);
        }

        void DataSource_UpdatingOrInserting(object sender, EntityDataSourceChangingEventArgs e)
        {
            MetaTable childTable = ChildrenColumn.ChildTable;

            // En los comentarios se asume un empleado o territorio a modo de ejemplo, pero el código es genérico

            // Obtener la colección de territorios de este empleado
            RelatedEnd entityCollection = (RelatedEnd)Column.EntityTypeProperty.GetValue(e.Entity, null);

            // En el modo de edición, asegurarse de que se ha cargado (no tiene sentido hacerlo en el modo de inserción)
            if (Mode == DataBoundControlMode.Edit && !entityCollection.IsLoaded)
            {
                entityCollection.Load();
            }

            // Obtener una IList de la colección (es decir, la lista de territorios del empleado actual)
            // REVISIÓN: debemos usar EntityCollection directamente, pero EF no tiene un
            // tipo que no sea genérico para esta colección. Se agregará en la siguiente versión
            IList entityList = ((IListSource)entityCollection).GetList();

            // Recorrer todos los territorios (no solo los de este empleado)
            foreach (object childEntity in childTable.GetQuery(e.Context))
            {

                // Comprobar si el empleado tiene actualmente este territorio
                bool isCurrentlyInList = entityList.Contains(childEntity);

                // Buscar la casilla para este territorio, que nos indica el nuevo estado
                string pkString = childTable.GetPrimaryKeyString(childEntity);
                ListItem listItem = CheckBoxList1.Items.FindByValue(pkString);
                if (listItem == null)
                    continue;

                // Si el estado es diferente, agregar o quitar según corresponda
                if (listItem.Selected)
                {
                    if (!isCurrentlyInList)
                        entityList.Add(childEntity);
                }
                else
                {
                    if (isCurrentlyInList)
                        entityList.Remove(childEntity);
                }
            }
        }

        protected void CheckBoxList1_DataBound(object sender, EventArgs e)
        {
            MetaTable childTable = ChildrenColumn.ChildTable;

            // En los comentarios se asume un empleado o territorio a modo de ejemplo, pero el código es genérico

            IList entityList = null;
            ObjectContext objectContext = null;

            if (Mode == DataBoundControlMode.Edit)
            {
                object entity;
                ICustomTypeDescriptor rowDescriptor = Row as ICustomTypeDescriptor;
                if (rowDescriptor != null)
                {
                    // Obtener la entidad real del contenedor
                    entity = rowDescriptor.GetPropertyOwner(null);
                }
                else
                {
                    entity = Row;
                }

                // Obtener la colección de territorios de este empleado y asegurarse de que se ha cargado
                RelatedEnd entityCollection = Column.EntityTypeProperty.GetValue(entity, null) as RelatedEnd;
                if (entityCollection == null)
                {
                    throw new InvalidOperationException(String.Format("La plantilla ManyToMany no admite el tipo de colección de la columna '{0}' en la tabla '{1}'.", Column.Name, Table.Name));
                }
                if (!entityCollection.IsLoaded)
                {
                    entityCollection.Load();
                }

                // Obtener una IList de la colección (es decir, la lista de territorios del empleado actual)
                // REVISIÓN: debemos usar EntityCollection directamente, pero EF no tiene un
                // tipo que no sea genérico para esta colección. Se agregará en la siguiente versión
                entityList = ((IListSource)entityCollection).GetList();

                // Obtener el ObjectContext actual
                // REVISIÓN: esta es una forma poco elegante de hacerlo. Buscar una alternativa mejor
                ObjectQuery objectQuery = (ObjectQuery)entityCollection.GetType().GetMethod(
                    "CreateSourceQuery").Invoke(entityCollection, null);
                objectContext = objectQuery.Context;
            }

            // Recorrer todos los territorios (no solo los de este empleado)
            foreach (object childEntity in childTable.GetQuery(objectContext))
            {
                MetaTable actualTable = MetaTable.GetTable(childEntity.GetType());
                // Crear una casilla para la colección
                ListItem listItem = new ListItem(
                    actualTable.GetDisplayString(childEntity),
                    actualTable.GetPrimaryKeyString(childEntity));

                // Hacer que se active si el empleado actual tiene ese territorio
                if (Mode == DataBoundControlMode.Edit)
                {
                    listItem.Selected = entityList.Contains(childEntity);
                }

                CheckBoxList1.Items.Add(listItem);
            }
        }

        public override Control DataControl
        {
            get
            {
                return CheckBoxList1;
            }
        }

    }
}
