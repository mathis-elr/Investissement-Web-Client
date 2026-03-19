import pandas as pd

chemin_fichier_a_modifier = "O:\\Mathis\\Documents\\prog perso\\c#\\Investissement_WebClient\\Euronext_Trackers.xlsx"
chemin_fichier_final = "O:\\Mathis\\Documents\\prog perso\\c#\\Investissement_WebClient\\etf_list.json"
df_fichier_a_modifier = pd.read_excel(chemin_fichier_a_modifier)

df_nv_nom_colonnes = df_fichier_a_modifier.rename(columns={"Instrument Fullname": "name", "Symbol" : "symbol"})
df_nv_nom_colonnes['symbol'] = df_nv_nom_colonnes['symbol'].astype(str) + ".PA"
maj_colonnes_etf_list = df_nv_nom_colonnes[["name", "ISIN", "symbol"]]

maj_colonnes_etf_list.to_json(chemin_fichier_final, orient='records')
