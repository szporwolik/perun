<!-- This is integration example for https://github.com/szporwolik/perun - please keep the code as simple and straightforward as possible -->
<!DOCTYPE html>
<html>
  <head>
    <title>Perun - Example of PHP Integration</title>
    <meta charset="utf-8">
    <meta name="description" content="Example of PHP integration of Perun for DCS World">
    <meta name="keywords" content="dcs world perun">
    <meta name="author" content="VladMordock">
	
	<style>
		body {
			background-color: #eeeeee;
			padding: 0 20px 0 20px;
		}

		h1,h2,h3 {
			color: navy; 
			
		}
		
		#header{
			padding: 10px 0 10px 0px;
		}
		
		#content{
			border-top: 1px solid navy;
			border-bottom: 1px solid navy;
			padding: 10px 0 10px 0px;
		}
		
		#footer{
			padding: 10px 0 10px 0px;
			text-align: right;
		}
		
		table, td{
			border: 1px solid navy;
		}
	</style>
  </head>
  <body>
	<div id="header">
		<h1>Perun Integration Example</h2>
		<h2>PHP Integration example for Perun for DCS World</h2>
		<h3>Get support at our <a href="https://discord.gg/MTahREx">Discord</a>
	</div>
	<div id="content">
		<?php
			// Configure database connection - put your database address and credentials here
				$config_db_username="CHANGE_ME";  // <<=== CHANGE TO YOUR MYSQL SERVER USERNAME
				$config_db_password="CHANGE_ME"; // <<=== CHANGE TO YOUR MYSQL SERVER PASSWORD
				$config_db_host="CHANGE_ME"; // <<=== CHANGE TO YOUR MYSQL SERVER ADDRESS
				$config_db_database="CHANGE_ME"; // <<=== CHANGE TO YOUR MYSQL DATABASE
			
			// Try to connect to the database
				$mysqli = new mysqli($config_db_host, $config_db_username, $config_db_password, $config_db_database);

			// Check connection
			if ($mysqli->connect_errno) {
				printf("Connect failed: %s\n", $mysqli->connect_error);
				exit();
			}

			// Some Examples
				// List last 5 missions at the server
				echo "<h3>List last 5 missions at the server</h3>";
				if ($result = $mysqli->query("SELECT * FROM `pe_DataMissionHashes` ORDER BY `pe_DataMissionHashes`.`pe_DataMissionHashes_datetime` DESC LIMIT 5")) {
					echo "<table>";
					while($row = mysqli_fetch_array($result))
					{
						echo "<tr>";
							echo "<td>" . $row['pe_DataMissionHashes_id'] . "</td>";
							echo "<td>" . $row['pe_DataMissionHashes_hash'] . "</td>";
							echo "<td>" . $row['pe_DataMissionHashes_datetime'] . "</td>";
						echo "</tr>";
					}
					echo "</table>";
					
					$result->close();
				}
				
				// List last 5
				echo "<h3>List last 5 players known server </h3>";
				if ($result = $mysqli->query("SELECT * FROM `pe_DataPlayers` ORDER BY `pe_DataPlayers`.`pe_DataPlayers_updated` DESC LIMIT 5")) {
					echo "<table>";
					while($row = mysqli_fetch_array($result))
					{
						echo "<tr>";
							echo "<td>" . $row['pe_DataPlayers_id'] . "</td>";
							echo "<td>" . $row['pe_DataPlayers_ucid'] . "</td>";
							echo "<td>" . $row['pe_DataPlayers_lastname'] . "</td>";
							echo "<td>" . $row['pe_DataPlayers_updated'] . "</td>";
						echo "</tr>";
					}
					echo "</table>";
					
					$result->close();
				}
				
				// List last 5 chat messages 
				echo "<h3>List last 5 chat messages (note: INNER JOIN is used to pull the player's name from other table)</h3>";
				if ($result = $mysqli->query("SELECT * FROM `pe_LogChat` INNER JOIN `pe_DataPlayers` on `pe_LogChat`.pe_LogChat_playerid = `pe_DataPlayers`.pe_DataPlayers_id ORDER BY `pe_LogChat`.`pe_LogChat_datetime` DESC LIMIT 5")) {
					echo "<table>";
					while($row = mysqli_fetch_array($result))
					{
						echo "<tr>";
							echo "<td>" . $row['pe_LogChat_datetime'] . "</td>";
							echo "<td>" . $row['pe_DataPlayers_lastname'] . "</td>";
							echo "<td>" . $row['pe_LogChat_msg'] . "</td>";
						echo "</tr>";
					}
					echo "</table>";
					
					$result->close();
				}
				
				
				// List last 5 events
				echo "<h3>List last 5 events</h3>";
				if ($result = $mysqli->query("SELECT * FROM `pe_LogEvent` ORDER BY `pe_LogEvent_datetime` DESC LIMIT 5")) {
					echo "<table>";
					while($row = mysqli_fetch_array($result))
					{
						echo "<tr>";
							echo "<td>" . $row['pe_LogEvent_datetime'] . "</td>";
							echo "<td>" . $row['pe_LogEvent_content'] . "</td>";
						echo "</tr>";
					}
					echo "</table>";
					
					$result->close();
				}
				
				// List last statistics
				echo "<h3>List last statistics (note: multiple INNER JOINs is used to pull information from other table)</h3>";
				if ($result = $mysqli->query("SELECT * FROM `pe_LogStats` INNER JOIN `pe_DataMissionHashes` ON `pe_LogStats`.pe_LogStats_missionhash_id = `pe_DataMissionHashes`.pe_DataMissionHashes_id INNER JOIN `pe_DataPlayers` ON `pe_LogStats`.pe_LogStats_playerid = `pe_DataPlayers`.pe_DataPlayers_id INNER JOIN `pe_DataTypes` ON `pe_LogStats`.pe_LogStats_typeid = `pe_DataTypes`.pe_DataTypes_id ORDER BY pe_LogStats_datetime desc LIMIT 5")) {
					echo "<table>";
					while($row = mysqli_fetch_array($result))
					{
						echo "<tr>";
							echo "<td>" . $row['pe_LogStats_datetime'] . "</td>";
							echo "<td>" . $row['pe_DataPlayers_lastname'] . "</td>";
							echo "<td>" . $row['pe_DataTypes_name'] . "</td>";
							echo "<td>" . $row['ps_crashes'] . "</td>";
							echo "<td>" . $row['ps_kills_planes'] . "</td>";
							echo "<td>" . $row['ps_kills_armor'] . "</td>";
							// TIP: See documentation there is many more counted events which can be displayed here
							echo "<td>" . $row['pe_LogStats_mstatus'] . "</td>";
						echo "</tr>";
					}
					echo "</table>";
					
					$result->close();
				}
				
				// Get JSON objects with actual server information including LotATC and DCS SRS information
				echo "<h3>Get JSON objects with actual server information including LotATC and DCS SRS information</h3>";
				if ($result = $mysqli->query("SELECT * FROM `pe_DataRaw`")) {
					echo "<table>";
					while($row = mysqli_fetch_array($result))
					{
						echo "<tr>";
							echo "<td>" . $row['pe_dataraw_updated'] . "</td>";
							echo "<td>" . $row['pe_dataraw_payload'] . "</td>";
						echo "</tr>";
					}
					echo "</table>";
					
					$result->close();
				}
			
			// Close database connection
				$mysqli->close();
		?>
	</div>
	<div id="footer">
		<a href="https://github.com/szporwolik/perun">
			<img src="https://camo.githubusercontent.com/50a1df184e069b36f3ddf9223bf16582f49aa16c/68747470733a2f2f692e696d6775722e636f6d2f5072496b714e412e706e67" alt="Perun_Logo">
		</a>
	</div>
  </body>
</html>