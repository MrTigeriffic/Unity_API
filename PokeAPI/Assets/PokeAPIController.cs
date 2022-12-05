using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using SimpleJSON;

public class PokeAPIController : MonoBehaviour
{
    [SerializeField] private RawImage pokeRawImage;
    [SerializeField] private TextMeshProUGUI pokeNameText, pokeNumText;
    [SerializeField] private TextMeshProUGUI[] pokeTypeTextArray;

    private readonly string basePokeURL = "https://pokeapi.co/api/v2/";

    // Start is called before the first frame update
    void Start()
    {
        pokeRawImage.texture = Texture2D.blackTexture;

        pokeNameText.text = "";
        pokeNumText.text = "";

        foreach (TextMeshProUGUI pokeTypeText in pokeTypeTextArray)
        {
            pokeTypeText.text = "";
        }
    }

    public void OnButtonRandomPokemon()
    {
        int randomPokeIndex = Random.Range(1, 808);

        pokeRawImage.texture = Texture2D.blackTexture;
        pokeNameText.text = "loading...";
        pokeNumText.text = "#" + randomPokeIndex;

        foreach (TextMeshProUGUI pokeTypeText in pokeTypeTextArray)
        {
            pokeTypeText.text = "";
        }

        StartCoroutine(GetPokemonAtIndex(randomPokeIndex));
    }

    IEnumerator GetPokemonAtIndex(int pokemonIndex)
    {
        string pokemonURL = basePokeURL + "pokemon/" + pokemonIndex.ToString();

        UnityWebRequest pokeInfoRequest = UnityWebRequest.Get(pokemonURL);

        yield return pokeInfoRequest.SendWebRequest();

        if(pokeInfoRequest.isNetworkError || pokeInfoRequest.isHttpError)
        {
            Debug.LogError(pokeInfoRequest.error);
            yield break;
        }

        JSONNode pokeInfo = JSON.Parse(pokeInfoRequest.downloadHandler.text);

        string pokeName = pokeInfo["name"];
        string pokeSpriteURL = pokeInfo["sprites"]["front_default"];

        JSONNode pokeTypes = pokeInfo["types"];
        string[] pokeTypeNames = new string[pokeTypes.Count];

        for (int i = 0, j = pokeTypes.Count - 1; i < pokeTypes.Count; i++, j--)
        {
            pokeTypeNames[j] = pokeTypes[i]["type"]["name"];
        }

        UnityWebRequest pokeSpriteRequest = UnityWebRequestTexture.GetTexture(pokeSpriteURL);
        yield return pokeSpriteRequest.SendWebRequest();

        if (pokeSpriteRequest.isNetworkError || pokeSpriteRequest.isHttpError)
        {
            Debug.LogError(pokeSpriteRequest.error);
            yield break;
        }

        pokeRawImage.texture = DownloadHandlerTexture.GetContent(pokeSpriteRequest);
        pokeRawImage.texture.filterMode = FilterMode.Point;

        pokeNameText.text = CapitalizeFirstLetter(pokeName);

        for (int i = 0; i < pokeTypeNames.Length; i++)
        {
            pokeTypeTextArray[i].text = CapitalizeFirstLetter(pokeTypeNames[i]);
        }

    }

    private string CapitalizeFirstLetter(string pokeName)
    {
        return char.ToUpper(pokeName[0]) + pokeName.Substring(1);
    }


}
