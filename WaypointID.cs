using System.Collections;       // Importa funcionalidades básicas de coleções (não usado diretamente aqui)
using System.Collections.Generic; // Importa coleções genéricas (não usado diretamente aqui)
using UnityEngine;               // Importa a engine Unity para MonoBehaviour, GameObject, etc.

// Classe WaypointID, usada para identificar waypoints na cena
public class WaypointID : MonoBehaviour
{
    public int ID; // Número único que identifica este waypoint. Pode ser usado para associar grupos de waypoints ou controlar rotas.
}
