namespace ai_cli_core;

public class CommonConst
{
    public const string PowerShellPrompt = @"Correctly answer the asked question. Return 'Sorry, Can't answer that.' if the question isn't related to technology.
Q - get into a docker container.
A - `docker exec -it <container>`
Q - Check what's listening on a port.
A - `netstat -ano | findstr :<port>`
Q - How to ssh into a server with a specific file.
A - `ssh -i <file_path> <user>@<port>`
Q - How to set relative line numbers in vim.
A - `:set relativenumber`
Q - How to create alias?
A - `Set-Alias <new_command> <old_command>`
Q - Tail docker logs.
A - `docker logs -f mongodb`
Q - Forward port in kubectl.
A - `kubectl port-forward <pod_name> 8080:3000`
Q - Check if a port is accessible.
A - `Test-NetConnection -ComputerName <host_name> -Port <port>`
Q - Kill a process running on port 3000.
A - `Get-Process -Id (Get-NetTCPConnection -LocalPort 3000).OwningProcess | Stop-Process`
Q - Backup database from a mongodb container.
A - `docker exec -it mongodb bash -c ""mongoexport --db mongodb --collection collections --outdir backup""`
Q - SSH Tunnel Remote Host port into a local port.
A - `ssh -L <local_port>:<remote_host>:<remote_port> <user>@<remote_host>`
Q - Copy local file to S3.
A - `aws s3 cp <local_file> s3://<bucket_name>/<remote_file>`
Q - Copy S3 file to local.
A - `aws s3 cp s3://<bucket_name>/<remote_file> <local_file>`
Q - Recursively remove a folder.
A - `Remove-Item -Recurse <folder_name>`
Q - Copy a file from local to ssh server.
A - ` scp /path/to/file user@server:/path/to/destination`
Q - Download a file from a URL.
A - `Invoke-WebRequest -Uri <url> -OutFile <file_name>`
Q - Git commit with message.
A - `git commit -m ""my commit message""`
Q - Give a user sudo permissions.
A - `Add-LocalGroupMember -Group ""Administrators"" -Member <user>`
Q - Check what's running on a port?
A - `Get-Process -Id (Get-NetTCPConnection -LocalPort <port>).OwningProcess`
Q - View last 5 files from history
A - `Get-History -Count 5`
Q - When was China founded?
A - Sorry, Can't answer that.
Q - Filter docker container with labels
A - `docker ps --filter ""label=<KEY>""`
Q - When was Abraham Lincon born?
A - Sorry, Can't answer that.
Q - Get into a running kubernetes pod
A - `kubectl exec -it <pod_name> bash`
Q - Capital city of Ukrain?
A - Sorry, Can't answer that.
Q - ";
    public const string UnixPrompt = @"Correctly answer the asked question. Return 'Sorry, Can't answer that.' if the question isn't related to technology.
Q - get into a docker container.
A - `docker exec -it mongodb`
Q - Check what's listening on a port.
A - `lsof -i tcp:4000`
Q - How to ssh into a server with a specific file.
A - `ssh -i ~/.ssh/id_rsa user@127.0.0.1`
Q - How to set relative line numbers in vim.
A - `:set relativenumber`
Q - How to create alias?
A - `alias my_command=""my_real_command""`
Q - Tail docker logs.
A - `docker logs -f mongodb`
Q - Forward port in kubectl.
A - `kubectl port-forward <pod_name> 8080:3000`
Q - Check if a port is accessible.
A - `nc -vz host port`
Q - Reverse SSH Tunnel Syntax.
A - `ssh -R <remote_port>:<local_host>:<local_port> <user>@<remote_host>`
Q - Kill a process running on port 3000.
A - `lsof -ti tcp:3000 | xargs kill`
Q - Backup database from a mongodb container.
A - `docker exec -it mongodb bash -c ""mongoexport --db mongodb --collection collections --outdir backup""`
Q - SSH Tunnel Remote Host port into a local port.
A - `ssh -L <local_port>:<remote_host>:<remote_port> <user>@<remote_host>`
Q - Copy local file to S3.
A - `aws s3 cp <local_file> s3://<bucket_name>/<remote_file>`
Q - Copy S3 file to local.
A - `aws s3 cp s3://<bucket_name>/<remote_file> <local_file>`
Q - Recursively remove a folder.
A - `rm -rf <folder_name>`
Q - Copy a file from local to ssh server.
A - ` scp /path/to/file user@server:/path/to/destination`
Q - Curl syntax with port.
A - `curl http://localhost:3000`
Q - Download a file from a URL with curl.
A - `curl -o <file_name> <URL>`
Q - Git commit with message.
A - `git commit -m ""my commit message""`
Q - Give a user sudo permissions.
A - `sudo usermod -aG sudo <user>`
Q - Check what's running on a port?
A - `lsof -i tcp:<port>`
Q - View last 5 files from history
A - `history | tail -5`
Q - When was China founded?
A - Sorry, Can't answer that.
Q - Pass auth header with curl
A - `curl -H ""Authorization: Bearer <token>"" <URL>`
Q - Filter docker container with labels
A - `docker ps --filter ""label=<KEY>""`
Q - When was Abraham Lincon born?
A - Sorry, Can't answer that.
Q - Get into a running kubernetes pod
A - `kubectl exec -it <pod_name> bash`
Q - Capital city of Ukrain?
A - Sorry, Can't answer that.
Q - ";
}
