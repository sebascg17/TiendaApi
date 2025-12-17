using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace TiendaApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GeografiaController : ControllerBase
    {
        // ============================================
        // DATA MOCK EXTENDIDA (Países de Habla Hispana)
        // ============================================

        private static readonly List<PaisData> _dataBase = new List<PaisData>
        {
            // SUDAMÉRICA
            new PaisData(1, "Colombia", "+57", new Dictionary<string, List<string>> {
                { "Amazonas", new List<string> { "Leticia", "Puerto Nariño" } },
                { "Antioquia", new List<string> { "Medellín", "Bello", "Itagüí", "Envigado", "Rionegro", "Sabaneta", "Apartadó" } },
                { "Arauca", new List<string> { "Arauca", "Saravena", "Tame" } },
                { "Atlántico", new List<string> { "Barranquilla", "Soledad", "Malambo", "Sabanalarga" } },
                { "Bogotá D.C.", new List<string> { "Bogotá D.C." } },
                { "Bolívar", new List<string> { "Cartagena", "Magangué", "Turbaco" } },
                { "Boyacá", new List<string> { "Tunja", "Duitama", "Sogamoso" } },
                { "Caldas", new List<string> { "Manizales", "La Dorada", "Chinchiná" } },
                { "Caquetá", new List<string> { "Florencia", "San Vicente del Caguán" } },
                { "Casanare", new List<string> { "Yopal", "Aguazul" } },
                { "Cauca", new List<string> { "Popayán", "Santander de Quilichao" } },
                { "Cesar", new List<string> { "Valledupar", "Aguachica" } },
                { "Chocó", new List<string> { "Quibdó", "Istmina" } },
                { "Córdoba", new List<string> { "Montería", "Lorica" } },
                { "Cundinamarca", new List<string> { "Soacha", "Zipaquirá", "Fusagasugá", "Facatativá", "Chía", "Mosquera" } },
                { "Guainía", new List<string> { "Inírida" } },
                { "Guaviare", new List<string> { "San José del Guaviare" } },
                { "Huila", new List<string> { "Neiva", "Pitalito" } },
                { "La Guajira", new List<string> { "Riohacha", "Maicao" } },
                { "Magdalena", new List<string> { "Santa Marta", "Ciénaga" } },
                { "Meta", new List<string> { "Villavicencio", "Acacías" } },
                { "Nariño", new List<string> { "Pasto", "Tumaco", "Ipiales" } },
                { "Norte de Santander", new List<string> { "Cúcuta", "Ocaña", "Villa del Rosario" } },
                { "Putumayo", new List<string> { "Mocoa", "Puerto Asís" } },
                { "Quindío", new List<string> { "Armenia", "Calarcá" } },
                { "Risaralda", new List<string> { "Pereira", "Dosquebradas" } },
                { "San Andrés y Providencia", new List<string> { "San Andrés", "Providencia" } },
                { "Santander", new List<string> { "Bucaramanga", "Floridablanca", "Girón", "Piedecuesta", "Barrancabermeja" } },
                { "Sucre", new List<string> { "Sincelejo", "Corozal" } },
                { "Tolima", new List<string> { "Ibagué", "Espinal" } },
                { "Valle del Cauca", new List<string> { "Cali", "Palmira", "Buenaventura", "Tuluá", "Buga", "Cartago" } },
                { "Vaupés", new List<string> { "Mitú" } },
                { "Vichada", new List<string> { "Puerto Carreño" } },
            }),
            new PaisData(2, "Argentina", "+54", new Dictionary<string, List<string>> {
                { "Buenos Aires", new List<string> { "La Plata", "Mar del Plata", "Bahía Blanca", "Merlo", "Quilmes" } },
                { "Ciudad Autónoma de Buenos Aires", new List<string> { "Buenos Aires" } },
                { "Catamarca", new List<string> { "San Fernando del Valle de Catamarca" } },
                { "Chaco", new List<string> { "Resistencia", "Sáenz Peña" } },
                { "Chubut", new List<string> { "Rawson", "Comodoro Rivadavia", "Trelew" } },
                { "Córdoba", new List<string> { "Córdoba", "Río Cuarto", "Villa Carlos Paz" } },
                { "Corrientes", new List<string> { "Corrientes", "Goya" } },
                { "Entre Ríos", new List<string> { "Paraná", "Concordia" } },
                { "Formosa", new List<string> { "Formosa", "Clorinda" } },
                { "Jujuy", new List<string> { "San Salvador de Jujuy" } },
                { "La Pampa", new List<string> { "Santa Rosa", "General Pico" } },
                { "La Rioja", new List<string> { "La Rioja" } },
                { "Mendoza", new List<string> { "Mendoza", "San Rafael" } },
                { "Misiones", new List<string> { "Posadas", "Oberá", "Puerto Iguazú" } },
                { "Neuquén", new List<string> { "Neuquén", "San Martín de los Andes" } },
                { "Río Negro", new List<string> { "Viedma", "Bariloche", "General Roca" } },
                { "Salta", new List<string> { "Salta" } },
                { "San Juan", new List<string> { "San Juan" } },
                { "San Luis", new List<string> { "San Luis", "Villa Mercedes" } },
                { "Santa Cruz", new List<string> { "Río Gallegos", "El Calafate" } },
                { "Santa Fe", new List<string> { "Santa Fe", "Rosario", "Rafaela" } },
                { "Santiago del Estero", new List<string> { "Santiago del Estero", "La Banda" } },
                { "Tierra del Fuego", new List<string> { "Ushuaia", "Río Grande" } },
                { "Tucumán", new List<string> { "San Miguel de Tucumán" } }
            }),
            new PaisData(3, "Chile", "+56", new Dictionary<string, List<string>> {
                 { "Santiago", new List<string> { "Santiago", "Puente Alto", "Maipú", "La Florida" } },
                 { "Valparaíso", new List<string> { "Valparaíso", "Viña del Mar", "Quilpué" } },
                 { "Biobío", new List<string> { "Concepción", "Talcahuano", "Los Ángeles" } },
                 { "Antofagasta", new List<string> { "Antofagasta", "Calama" } },
                 { "Araucanía", new List<string> { "Temuco", "Padre Las Casas" } },
                 { "Coquimbo", new List<string> { "La Serena", "Coquimbo" } },
                 { "Los Lagos", new List<string> { "Puerto Montt", "Osorno" } },
                 { "O'Higgins", new List<string> { "Rancagua" } },
                 { "Maule", new List<string> { "Talca", "Curicó" } }
            }),
             new PaisData(4, "Perú", "+51", new Dictionary<string, List<string>> {
                 { "Lima", new List<string> { "Lima", "Callao" /*Callao es prov const pero usualmente se agrupa en listas simples*/ } },
                 { "Arequipa", new List<string> { "Arequipa" } },
                 { "La Libertad", new List<string> { "Trujillo" } },
                 { "Lambayeque", new List<string> { "Chiclayo" } },
                 { "Piura", new List<string> { "Piura", "Sullana" } },
                 { "Junín", new List<string> { "Huancayo" } },
                 { "Cusco", new List<string> { "Cusco" } },
                 { "Loreto", new List<string> { "Iquitos" } }
            }),
             new PaisData(5, "Ecuador", "+593", new Dictionary<string, List<string>> {
                 { "Pichincha", new List<string> { "Quito", "Santo Domingo" } },
                 { "Guayas", new List<string> { "Guayaquil", "Durán" } },
                 { "Azuay", new List<string> { "Cuenca" } },
                 { "Manabí", new List<string> { "Portoviejo", "Manta" } },
                 { "El Oro", new List<string> { "Machala" } },
                 { "Loja", new List<string> { "Loja" } }
            }),
             new PaisData(6, "Venezuela", "+58", new Dictionary<string, List<string>> {
                 { "Distrito Capital", new List<string> { "Caracas" } },
                 { "Zulia", new List<string> { "Maracaibo", "Cabimas" } },
                 { "Carabobo", new List<string> { "Valencia", "Puerto Cabello" } },
                 { "Lara", new List<string> { "Barquisimeto" } },
                 { "Aragua", new List<string> { "Maracay" } },
                 { "Bolívar", new List<string> { "Ciudad Guayana", "Ciudad Bolívar" } },
                 { "Anzoátegui", new List<string> { "Barcelona", "Puerto La Cruz" } }
            }),
             new PaisData(7, "Bolivia", "+591", new Dictionary<string, List<string>> {
                 { "La Paz", new List<string> { "La Paz", "El Alto" } },
                 { "Santa Cruz", new List<string> { "Santa Cruz de la Sierra" } },
                 { "Cochabamba", new List<string> { "Cochabamba" } },
                 { "Oruro", new List<string> { "Oruro" } },
                 { "Potosí", new List<string> { "Potosí" } },
                 { "Tarija", new List<string> { "Tarija" } },
                 { "Chuquisaca", new List<string> { "Sucre" } },
                 { "Beni", new List<string> { "Trinidad" } },
                 { "Pando", new List<string> { "Cobija" } }
            }),
             new PaisData(8, "Paraguay", "+595", new Dictionary<string, List<string>> {
                 { "Asunción", new List<string> { "Asunción" } },
                 { "Alto Paraná", new List<string> { "Ciudad del Este" } },
                 { "Central", new List<string> { "Luque", "San Lorenzo", "Capiatá" } },
                 { "Itapúa", new List<string> { "Encarnación" } }
            }),
             new PaisData(9, "Uruguay", "+598", new Dictionary<string, List<string>> {
                 { "Montevideo", new List<string> { "Montevideo" } },
                 { "Canelones", new List<string> { "Canelones", "Ciudad de la Costa" } },
                 { "Maldonado", new List<string> { "Maldonado", "Punta del Este" } },
                 { "Salto", new List<string> { "Salto" } },
                 { "Colonia", new List<string> { "Colonia del Sacramento" } }
            }),
             // NORTE Y CENTRO AMÉRICA
            new PaisData(10, "México", "+52", new Dictionary<string, List<string>> {
                 { "Ciudad de México", new List<string> { "Ciudad de México" } },
                 { "Jalisco", new List<string> { "Guadalajara", "Zapopan", "Puerto Vallarta" } },
                 { "Nuevo León", new List<string> { "Monterrey", "San Pedro Garza García" } },
                 { "Puebla", new List<string> { "Puebla" } },
                 { "Guanajuato", new List<string> { "León", "Guanajuato", "Irapuato" } },
                 { "Baja California", new List<string> { "Tijuana", "Mexicali", "Ensenada" } },
                 { "México", new List<string> { "Ecatepec", "Naucalpan", "Toluca" } },
                 { "Veracruz", new List<string> { "Veracruz", "Xalapa" } },
                 { "Yucatán", new List<string> { "Mérida" } },
                 { "Quintana Roo", new List<string> { "Cancún", "Playa del Carmen" } },
                 // ... Se pueden agregar más de los 32 estados
            }),
            new PaisData(11, "España", "+34", new Dictionary<string, List<string>> {
                 { "Madrid", new List<string> { "Madrid", "Móstoles", "Alcalá de Henares" } },
                 { "Cataluña", new List<string> { "Barcelona", "L'Hospitalet de Llobregat" } },
                 { "Andalucía", new List<string> { "Sevilla", "Málaga", "Córdoba", "Granada" } },
                 { "Comunidad Valenciana", new List<string> { "Valencia", "Alicante" } },
                 { "Galicia", new List<string> { "A Coruña", "Vigo", "Santiago de Compostela" } },
                 { "País Vasco", new List<string> { "Bilbao", "San Sebastián", "Vitoria" } },
                 { "Castilla y León", new List<string> { "Valladolid", "Burgos", "Salamanca" } }
            }),
            new PaisData(12, "Guatemala", "+502", new Dictionary<string, List<string>> { { "Guatemala", new List<string> { "Ciudad de Guatemala", "Mixco" } }, { "Quetzaltenango", new List<string> { "Quetzaltenango" } }, { "Escuintla", new List<string> { "Escuintla" } } }),
            new PaisData(13, "Costa Rica", "+506", new Dictionary<string, List<string>> { { "San José", new List<string> { "San José" } }, { "Alajuela", new List<string> { "Alajuela" } }, { "Heredia", new List<string> { "Heredia" } } }),
            new PaisData(14, "Panamá", "+507", new Dictionary<string, List<string>> { { "Panamá", new List<string> { "Ciudad de Panamá" } }, { "Chiriquí", new List<string> { "David" } }, { "Colón", new List<string> { "Colón" } } }),
            new PaisData(15, "República Dominicana", "+1", new Dictionary<string, List<string>> { { "Distrito Nacional", new List<string> { "Santo Domingo" } }, { "Santiago", new List<string> { "Santiago de los Caballeros" } }, { "La Altagracia", new List<string> { "Punta Cana" } } }),
            new PaisData(16, "Cuba", "+53", new Dictionary<string, List<string>> { { "La Habana", new List<string> { "La Habana" } }, { "Santiago de Cuba", new List<string> { "Santiago de Cuba" } } }),
            new PaisData(17, "El Salvador", "+503", new Dictionary<string, List<string>> { { "San Salvador", new List<string> { "San Salvador" } }, { "La Libertad", new List<string> { "Santa Tecla" } } }),
            new PaisData(18, "Honduras", "+504", new Dictionary<string, List<string>> { { "Francisco Morazán", new List<string> { "Tegucigalpa" } }, { "Cortés", new List<string> { "San Pedro Sula" } } }),
            new PaisData(19, "Nicaragua", "+505", new Dictionary<string, List<string>> { { "Managua", new List<string> { "Managua" } }, { "León", new List<string> { "León" } } }),
            new PaisData(20, "Puerto Rico", "+1", new Dictionary<string, List<string>> { { "San Juan", new List<string> { "San Juan" } }, { "Bayamón", new List<string> { "Bayamón" } } }),
            new PaisData(21, "Estados Unidos", "+1", new Dictionary<string, List<string>> { 
                { "California", new List<string> { "Los Angeles", "San Francisco" } },
                { "Florida", new List<string> { "Miami", "Orlando" } },
                { "Texas", new List<string> { "Houston", "Austin" } },
                { "New York", new List<string> { "New York City" } }
            }) // Incluido por alta población hispana
        };

        // GET: api/geografia/paises
        [HttpGet("paises")]
        public IActionResult GetPaises()
        {
            var paises = _dataBase
                .Select(p => new { p.Id, p.Nombre, p.Codigo })
                .OrderByDescending(p => p.Nombre == "Colombia") // True (1) primero, False (0) después
                .ThenBy(p => p.Nombre)
                .ToList();
            return Ok(paises);
        }

        // GET: api/geografia/departamentos/{paisId}
        [HttpGet("departamentos/{paisId}")]
        public IActionResult GetDepartamentos(int paisId)
        {
            var pais = _dataBase.FirstOrDefault(p => p.Id == paisId);
            if (pais == null) return NotFound("País no encontrado");

            int idCounter = paisId * 1000;
            var deptos = pais.Regiones.Keys.Select(nombre => new { Id = idCounter++, Nombre = nombre }).OrderBy(d => d.Nombre).ToList();
            
            return Ok(deptos);
        }

        // GET: api/geografia/municipios/{deptoId}
        // NOTA: En este mock simplificado, deptoId es un int generado dinámicamente.
        // Como no persiste, vamos a tener que "adivinar" el nombre o pasar el nombre desde el front.
        // PERO para mantener compatibilidad con el front actual que envía ID, vamos a hacer un truco de búsqueda inversa o Hash simple.
        // MEJOR APROXIMACIÓN PARA MOCK: Recibir paisId y NombreDepto sería ideal, pero el front manda ID.
        // HACK: El ID generado fue (paisId * 1000) + índice. 
        // Vamos a reconstruirlo.
        [HttpGet("municipios/{deptoId}")]
        public IActionResult GetMunicipios(int deptoId)
        {
            int paisId = deptoId / 1000;
            int indiceDepto = deptoId % 1000;
            
            var pais = _dataBase.FirstOrDefault(p => p.Id == paisId);
            if (pais == null) return NotFound("País/Dpto no encontrado");

            // Reconstruir el nombre basado en el orden alfabético usado en GetDepartamentos
            var deptosOrdenados = pais.Regiones.Keys.OrderBy(k => k).ToList();
            
            // Los IDs asignados empezaron en (paisId * 1000) -> indice 0
            int index = deptoId - (paisId * 1000);

            if (index < 0 || index >= deptosOrdenados.Count) return NotFound("Departamento no encontrado");

            string nombreDepto = deptosOrdenados[index];
            if (!pais.Regiones.ContainsKey(nombreDepto)) return Ok(new List<dynamic>());

            var ciudades = pais.Regiones[nombreDepto];
            
            int ciudadIdBase = deptoId * 100;
            var result = ciudades.Select(( nombre, i) => new { Id = ciudadIdBase + i, Nombre = nombre }).OrderBy(c => c.Nombre).ToList();

            return Ok(result);
        }

        // Clase auxiliar interna para estructura de datos
        private class PaisData
        {
            public int Id { get; set; }
            public string Nombre { get; set; }
            public string Codigo { get; set; }
            public Dictionary<string, List<string>> Regiones { get; set; }

            public PaisData(int id, string nombre, string codigo, Dictionary<string, List<string>> regiones)
            {
                Id = id;
                Nombre = nombre;
                Codigo = codigo;
                Regiones = regiones;
            }
        }
    }
}
