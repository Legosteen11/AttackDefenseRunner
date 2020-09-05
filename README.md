# AttackDefenseRunner
Runner for A&amp;D CTFs

# Structure
1. Give ADR the flag regex
2. Give available attack servers
3. Give vulnerable hosts
4. Upload the docker tag of your docker image
    1. Choose server to run the docker image
        1. Every docker image should ideally have it's own server
        2. If there are not enough servers available, distribute the docker images as good as possible
    2. Make that server run the docker image
    3. Filter the output of the docker image with a given regex (still on the remote server)
    4. Send flags back with RabbitMQ
5. Log everything

# Writing an exploit
1. Use pre-given files (which manage going through every vulnerable host)
2. Write an exploit which exploits 1 vulnerable host
3. Build the docker image, including the pre-given files
4. Give the docker tag to ADR
