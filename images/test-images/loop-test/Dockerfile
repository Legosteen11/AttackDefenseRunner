FROM deltaclock/pwntools:latest

WORKDIR /usr/src/app

COPY . .
RUN pip install --no-cache-dir -r requirements.txt

CMD ["bash","looper.sh","/tmp/adr/targets"]