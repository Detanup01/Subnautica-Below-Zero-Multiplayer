﻿namespace Subnautica.Client.MonoBehaviours.Player
{
    using System.Collections.Generic;

    using Subnautica.API.Features;

    using UnityEngine;
    using UnityEngine.Rendering;

    public class PlayerEquipment : MonoBehaviour
    {
        private Dictionary<TechType, List<GameObject>> BodyModels { get; set; } = new Dictionary<TechType, List<GameObject>>();

        private Dictionary<TechType, List<GameObject>> GlovesModel { get; set; } = new Dictionary<TechType, List<GameObject>>();

        private Dictionary<TechType, List<GameObject>> HeadModels { get; set; } = new Dictionary<TechType, List<GameObject>>();

        private GameObject FootModel { get; set; }

        private List<Renderer> HeadRenderers { get; set; } = new List<Renderer>();

        public ZeroPlayer Player { get; set; }

        public List<TechType> OldEquipments { get; set; } = new List<TechType>()
        {
            TechType.None,
            TechType.None,
            TechType.None,
            TechType.None,
            TechType.None,
        };

        public void Awake()
        {
            this.AddModel(this.BodyModels, TechType.None, GameIndex.PLAYER_MODEL_BODY_DEFAULT);
            this.AddModel(this.BodyModels, TechType.ColdSuit, GameIndex.PLAYER_MODEL_BODY_COLD_SUIT);
            this.AddModel(this.BodyModels, TechType.ReinforcedDiveSuit, GameIndex.PLAYER_MODEL_BODY_REINFORCED_SUIT);
            this.AddModel(this.BodyModels, TechType.WaterFiltrationSuit, GameIndex.PLAYER_MODEL_BODY_WATER_SUIT);

            this.AddModel(this.GlovesModel, TechType.None, new string[] { GameIndex.PLAYER_MODEL_HAND_DEFAULT, GameIndex.PLAYER_MODEL_GLOVES_DEFAULT });
            this.AddModel(this.GlovesModel, TechType.ReinforcedGloves, GameIndex.PLAYER_MODEL_GLOVES_REINFORCED);
            this.AddModel(this.GlovesModel, TechType.ColdSuitGloves, GameIndex.PLAYER_MODEL_GLOVES_COLD_SUIT);

            this.AddModel(this.HeadModels, TechType.None, GameIndex.PLAYER_MODEL_HEAD_DEFAULT);
            this.AddModel(this.HeadModels, TechType.Rebreather, GameIndex.PLAYER_MODEL_HEAD_DEFAULT);
            this.AddModel(this.HeadModels, TechType.ColdSuitHelmet, new string[] { GameIndex.PLAYER_MODEL_HEAD_COLD_SUIT, GameIndex.PLAYER_MODEL_MASK_COLD_SUIT });
            this.AddModel(this.HeadModels, TechType.FlashlightHelmet, new string[] { GameIndex.PLAYER_MODEL_HEAD_DEFAULT, GameIndex.PLAYER_MODEL_HEAD_FLASH_LIGHT });

            this.HeadRenderers.AddRange(this.GetModelRenderers(GameIndex.PLAYER_MODEL_HEAD_DEFAULT));
            this.HeadRenderers.AddRange(this.GetModelRenderers(GameIndex.PLAYER_MODEL_MASK_COLD_SUIT));
            this.HeadRenderers.AddRange(this.GetModelRenderers(GameIndex.PLAYER_MODEL_HEAD_FLASH_LIGHT));

            this.FootModel = this.GetGameObject(GameIndex.PLAYER_MODEL_FLIPPER_FINS);
        }

        public void FixedUpdate()
        {
            for (int i = 0; i < this.Player.Equipments.Count; i++)
            {
                if (this.Player.Equipments[i] == this.OldEquipments[i])
                {
                    continue;
                }

                this.OldEquipments[i] = this.Player.Equipments[i];

                switch (i)
                {
                    case 0: 
                        this.ChangeHeadModel(this.OldEquipments[i]);
                        break;
                    case 1:
                        this.ChangeBodyModel(this.OldEquipments[i]);
                        break;
                    case 2:
                        this.ChangeHandModel(this.OldEquipments[i]);
                        break;
                    case 3:
                        this.ChangeFootModel(this.OldEquipments[i]);
                        break;
                    case 4:
                        break;
                }
            }
        }

        public void ResetEquipments()
        {
            for (int i = 0; i < this.Player.Equipments.Count; i++)
            {
                this.Player.Equipments[i] = TechType.None;
            }

            this.ChangeHeadModel(TechType.None);
            this.ChangeBodyModel(TechType.None);
            this.ChangeHandModel(TechType.None);
            this.ChangeFootModel(TechType.None);
        }

        public void ChangeFootModel(TechType techType)
        {
            if (this.FootModel != null)
            {
                switch (techType)
                {
                    case TechType.Fins:
                    case TechType.SwimChargeFins:
                    case TechType.UltraGlideFins:
                        this.FootModel.SetActive(true);
                        break;
                    default:
                        this.FootModel.SetActive(false);
                        break;
                }
            }
        }

        public void ChangeHeadModel(TechType techType)
        {
            this.ChangeModelStatus(this.HeadModels, techType);

            foreach (Renderer renderer in this.HeadRenderers)
            {
                if (this.HeadModels.ContainsKey(techType))
                {
                    renderer.shadowCastingMode = ShadowCastingMode.On;
                }
                else
                {
                    renderer.shadowCastingMode = ShadowCastingMode.ShadowsOnly;
                }
            }
        }

        public void ChangeBodyModel(TechType techType)
        {
            this.ChangeModelStatus(this.BodyModels, techType);
        }

        public void ChangeHandModel(TechType techType)
        {
            this.ChangeModelStatus(this.GlovesModel, techType);
        }

        private void ChangeModelStatus(Dictionary<TechType, List<GameObject>> models, TechType techType)
        {
            var actives = new List<GameObject>();

            foreach (var model in models)
            {
                if (model.Value == null)
                {
                    continue;
                }

                if (model.Key == techType)
                {
                    foreach (var item in model.Value)
                    {
                        actives.Add(item);
                        item.SetActive(true);
                    }
                }
                else
                {
                    foreach (var item in model.Value)
                    {
                        if (!actives.Contains(item))
                        {
                            item.SetActive(false);
                        }
                    }
                }
            }

            actives.Clear();
        }

        private void AddModel(Dictionary<TechType, List<GameObject>> models, TechType techType, string path)
        {
            this.AddModel(models, techType, new string[] { path });
        }

        private void AddModel(Dictionary<TechType, List<GameObject>> models, TechType techType, string[] paths)
        {
            var gameObjects = new List<GameObject>();
            foreach (string path in paths)
            {
                var transform = this.GetGameObject(path);
                if (transform == null)
                {
                    continue;
                }

                gameObjects.Add(transform.gameObject);
            }

            if (gameObjects.Count > 0)
            {
                models.Add(techType, gameObjects);
            }
        }

        private GameObject GetGameObject(string path)
        {
            var transform = this.transform.Find(path);
            if (transform == null)
            {
                return null;
            }

            return transform.gameObject;
        }

        private Renderer[] GetModelRenderers(string path)
        {
            var transform = this.GetGameObject(path);
            if (transform == null)
            {
                return null;
            }

            return transform.GetComponentsInChildren<Renderer>();
        }
    }
}
