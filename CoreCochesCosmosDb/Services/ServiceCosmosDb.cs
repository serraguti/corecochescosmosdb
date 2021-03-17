using CoreCochesCosmosDb.Models;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CoreCochesCosmosDb.Services
{
    public class ServiceCosmosDb
    {
        //TODO FUNCIONA CON UN CLIENT DE COSMOS
        //HEMOS CREADO UNA CUENTA EN UN ENDPOINT LLAMADA miscochespgs
        //DENTRO DE ESTA CUENTA SE CREAN DATABASES
        //DENTRO DE LA BBDD SE CREAN COLLECTION
        //DENTRO DE LAS COLECCIONES IRAN LOS DOCUMENTSDB (JSON)
        DocumentClient client;
        String bbdd;
        String collection;

        public ServiceCosmosDb(IConfiguration configuration)
        {
            String endpoint = configuration["CosmosDb:endpoint"];
            String primarykey = configuration["CosmosDb:primarykey"];
            this.bbdd = "Vehiculos BBDD";
            this.collection = "VehiculosCollection";
            this.client =
                new DocumentClient(new Uri(endpoint), primarykey);
        }

        public async Task CrearBbddVehiculosAsync()
        {
            Database bbdd = new Database() { Id = this.bbdd };
            await this.client.CreateDatabaseAsync(bbdd);
        }

        public async Task CrearColeccionVehiculosAsync()
        {
            DocumentCollection coleccion =
                new DocumentCollection() { Id = this.collection };
            await this.client.CreateDocumentCollectionAsync
                (UriFactory.CreateDatabaseUri(this.bbdd), coleccion);
        }

        public async Task InsertarVehiculo(Vehiculo car)
        {
            //RECUPERAMOS LA URI PARA LA COLECCION
            //DONDE IRA EL VEHICULO
            Uri uri =
                UriFactory.CreateDocumentCollectionUri(this.bbdd, this.collection);
            await this.client.CreateDocumentAsync(uri, car);
        }

        public List<Vehiculo> GetVehiculos()
        {
            //DEBEMOS INDICAR EL NUMERO DE ELEMENTOS A RECUPERAR
            FeedOptions options = new FeedOptions() { MaxItemCount = -1 };
            String sql = "SELECT * FROM c";
            Uri uri =
                UriFactory.CreateDocumentCollectionUri(this.bbdd, this.collection);
            IQueryable<Vehiculo> consulta =
                this.client.CreateDocumentQuery<Vehiculo>(uri, sql, options);
            return consulta.ToList();
        }

        public async Task<Vehiculo> FindVehiculoAsync(String id)
        {
            Uri uri =
                UriFactory.CreateDocumentUri(this.bbdd, this.collection, id);
            Document document = await this.client.ReadDocumentAsync(uri);
            //ESTE DOCUMENTO ES UN STREAM
            MemoryStream memory = new MemoryStream();
            using (var stream = new StreamReader(memory))
            {
                //ALMACENAMOS EL DOCUMENTO EN LA MEMORIA
                document.SaveTo(memory);
                //PONEMOS LA MEMORY A ZERO
                memory.Position = 0;
                //DESERIALIZAMOS CON JSONCONVERT
                Vehiculo car =
                    JsonConvert.DeserializeObject<Vehiculo>
                    (await stream.ReadToEndAsync());
                return car;
            }
        }

        public async Task ModificarVehiculo(Vehiculo car)
        {
            Uri uri =
                UriFactory.CreateDocumentUri(this.bbdd, this.collection
                , car.Id);
            await this.client.ReplaceDocumentAsync(uri, car);
        }

        public async Task EliminarVehiculo(String id)
        {
            Uri uri =
                UriFactory.CreateDocumentUri(this.bbdd, this.collection, id);
            await this.client.DeleteDocumentAsync(uri);
        }

        public List<Vehiculo> BuscarVehiculosMarca(String marca)
        {
            FeedOptions options = new FeedOptions() { MaxItemCount = -1 };
            Uri uri =
                UriFactory.CreateDocumentCollectionUri(this.bbdd, this.collection);
            String sql = "select * from c where c.Marca = '" + marca + "'";
            IQueryable<Vehiculo> query =
                this.client.CreateDocumentQuery<Vehiculo>(uri, sql, options);
            IQueryable<Vehiculo> querylambda =
                this.client.CreateDocumentQuery<Vehiculo>(uri, options)
                .Where(z => z.Marca == marca);
            return query.ToList();
        }

        public List<Vehiculo> CrearCoches()
        {
            List<Vehiculo> coches = new List<Vehiculo>()
            {
                 new Vehiculo
                 {
                   Id = "1", Marca = "Pontiac", Modelo = "FireBird"
                   , Motor = new Motor { Tipo = "Gasolina", Caballos = 240, Potencia = 140}
                   , VelocidadMaxima = 250
                 },
                 new Vehiculo
                 {
                   Id = "2", Marca = "Audi", Modelo = "A6"
                   , Motor = new Motor { Tipo = "Diesel", Caballos = 270, Potencia = 150}
                   , VelocidadMaxima = 270
                 },
                 new Vehiculo
                 {
                   Id = "3", Marca = "Powell Peralta", Modelo = "Patinete"
                   , VelocidadMaxima = 20
                 }
            };
            return coches;
        }
    }
}
